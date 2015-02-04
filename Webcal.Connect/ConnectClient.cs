namespace Webcal.Connect
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using Shared;
    using Webcal.Shared.Connect;

    public class ConnectClient : BaseConnectClient, IConnectClient
    {
        private ChannelFactory<IConnectService> _channelFactory;

        public bool IsOpen
        {
            get { return Service != null; }
        }

        public IConnectOperationResult Open(IConnectKeys connectKeys)
        {
            return Try(() =>
            {
                Binding binding = new ConnectBindingHelper().CreateBinding(new ConnectTokenParameters());
                var serviceAddress = new EndpointAddress(connectKeys.Url);

                _channelFactory = new ChannelFactory<IConnectService>(binding, serviceAddress);

                var credentials = new ConnectClientCredentials(connectKeys);
                credentials.ServiceCertificate.SetDefaultCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindByIssuerName, new Uri(connectKeys.Url).Host);

                _channelFactory.Endpoint.Behaviors.Remove(typeof (ClientCredentials));
                _channelFactory.Endpoint.Behaviors.Add(credentials);

                Service = _channelFactory.CreateChannel();
                Service.Echo();
            });
        }

        public IConnectService Service { get; set; }

        public override void ForceClose()
        {
            if (_channelFactory != null)
                _channelFactory.Abort();
        }

        public void Close()
        {
            if(_channelFactory != null)
                _channelFactory.Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}