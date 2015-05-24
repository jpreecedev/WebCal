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
            if (_channelFactory != null)
            {
                if (_channelFactory.State == CommunicationState.Opened || _channelFactory.State == CommunicationState.Opening)
                {
                    return;
                }
            }

            var serviceAddress = new EndpointAddress(connectKeys.Url);

            _channelFactory = new ChannelFactory<IConnectService>(GetBinding(), serviceAddress);

            var credentials = new ConnectClientCredentials(connectKeys);
            credentials.ServiceCertificate.DefaultCertificate = GetCertificate();

            _channelFactory.Endpoint.Behaviors.Remove(typeof(ClientCredentials));
            _channelFactory.Endpoint.Behaviors.Add(credentials);

            Service = _channelFactory.CreateChannel();
        }
        
        public void Close()
        {
            if (_channelFactory != null)
            {
                _channelFactory.Abort();
                _channelFactory.Close();
            }
        }

        public void Dispose()
        {
            Close();
        }

        private static Binding GetBinding()
        {
#if DEBUG
            return new ConnectBindingHelper().CreateBinding();
#else
            return new ConnectBindingHelper().CreateHttpsBinding();
#endif
        }

        private static X509Certificate2 GetCertificate()
        {
#if DEBUG
            return new X509Certificate2(Resources.webcal_connect);
#else
            return new X509Certificate2(Resources.webcalconnect_com);            
#endif
        }
    }
}