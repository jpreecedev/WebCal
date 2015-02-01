namespace Webcal.Connect
{
    using System;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using Shared;
    using Webcal.Shared.Connect;

    public class ConnectTokenProvider : SecurityTokenProvider
    {
        private readonly IConnectKeys _connectKeys;

        public ConnectTokenProvider(IConnectKeys connectKeys)
        {
            if (connectKeys == null)
            {
                throw new ArgumentNullException("connectKeys");
            }
            _connectKeys = connectKeys;
        }

        protected override SecurityToken GetTokenCore(TimeSpan timeout)
        {
            return new ConnectToken(_connectKeys);
        }
    }
}