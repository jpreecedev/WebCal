namespace TachographReader.ConnectClient
{
    using System;
    using System.IdentityModel.Selectors;
    using System.ServiceModel.Description;
    using Connect.Shared;

    public class ConnectClientCredentials : ClientCredentials
    {
        private readonly IConnectKeys _connectKeys;

        public ConnectClientCredentials(IConnectKeys connectKeys)
        {
            if (connectKeys == null)
            {
                throw new ArgumentNullException("connectKeys");
            }
            _connectKeys = connectKeys;
        }

        public IConnectKeys ConnectKeys
        {
            get { return _connectKeys; }
        }

        protected override ClientCredentials CloneCore()
        {
            return new ConnectClientCredentials(_connectKeys);
        }

        public override SecurityTokenManager CreateSecurityTokenManager()
        {
            return new ConnectClientCredentialsSecurityTokenManager(this);
        }
    }
}