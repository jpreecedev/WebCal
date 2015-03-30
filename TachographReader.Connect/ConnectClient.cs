namespace TachographReader.ConnectClient
{
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using Connect.Shared;
    using Properties;
    using Shared.Connect;

    public class ConnectClient : BaseConnectClient, IConnectClient
    {
        private ChannelFactory<IConnectService> _channelFactory;

        public bool IsOpen
        {
            get { return Service != null; }
        }

        public IConnectService Service { get; set; }

        public void Open(IConnectKeys connectKeys)
        {
            Binding binding = new ConnectBindingHelper().CreateBinding(new ConnectTokenParameters());
            var serviceAddress = new EndpointAddress(connectKeys.Url);

            _channelFactory = new ChannelFactory<IConnectService>(binding, serviceAddress);

            var credentials = new ConnectClientCredentials(connectKeys);
            var certificate = new X509Certificate2(Resources.webcal_connect);
            credentials.ServiceCertificate.DefaultCertificate = certificate;

            _channelFactory.Endpoint.Behaviors.Remove(typeof(ClientCredentials));
            _channelFactory.Endpoint.Behaviors.Add(credentials);

            Service = _channelFactory.CreateChannel();
        }
        
        public override void ForceClose()
        {
            if (_channelFactory != null)
                _channelFactory.Abort();
        }

        public void Close()
        {
            if (_channelFactory != null)
                _channelFactory.Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}