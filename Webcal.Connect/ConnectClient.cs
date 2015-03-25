namespace Webcal.ConnectClient
{
    using System;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using Connect.Shared;
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
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
    ((sender, certificate, chain, sslPolicyErrors) => true);

// trust sender
System.Net.ServicePointManager.ServerCertificateValidationCallback
                = ((sender, cert, chain, errors) => cert.Subject.Contains("YourServerName"));

// validate cert by calling a function
ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);

            Binding binding = new ConnectBindingHelper().CreateBinding(new ConnectTokenParameters());
            var serviceAddress = new EndpointAddress(connectKeys.Url);

            _channelFactory = new ChannelFactory<IConnectService>(binding, serviceAddress);

            var credentials = new ConnectClientCredentials(connectKeys);
            credentials.ServiceCertificate.SetDefaultCertificate(StoreLocation.CurrentUser, StoreName.My, X509FindType.FindBySubjectName,"webcalconnect.com");

            _channelFactory.Endpoint.Behaviors.Remove(typeof(ClientCredentials));
            _channelFactory.Endpoint.Behaviors.Add(credentials);

            Service = _channelFactory.CreateChannel();
        }

        // callback used to validate the certificate in an SSL conversation
        private static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
        {
            bool result = false;
            if (cert.Subject.ToUpper().Contains("YourServerName"))
            {
                result = true;
            }

            return result;
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