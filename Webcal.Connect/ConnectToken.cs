namespace Webcal.Connect
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IdentityModel.Tokens;
    using Shared.Connect;

    public class ConnectToken : SecurityToken
    {
        private readonly IConnectKeys _connectKeys;
        private readonly DateTime _effectiveTime = DateTime.UtcNow;
        private readonly string _id;
        private readonly ReadOnlyCollection<SecurityKey> _securityKeys;

        public ConnectToken(IConnectKeys cardInfo)
            : this(cardInfo, Guid.NewGuid().ToString())
        {
        }

        public ConnectToken(IConnectKeys connectKeys, string id)
        {
            if (connectKeys == null)
            {
                throw new ArgumentNullException("connectKeys");
            }
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            _connectKeys = connectKeys;
            _id = id;
            _securityKeys = new ReadOnlyCollection<SecurityKey>(new List<SecurityKey>());
        }

        public IConnectKeys ConnectKeys
        {
            get { return _connectKeys; }
        }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get { return _securityKeys; }
        }

        public override DateTime ValidFrom
        {
            get { return _effectiveTime; }
        }

        public override DateTime ValidTo
        {
            get { return DateTime.MaxValue; }
        }

        public override string Id
        {
            get { return _id; }
        }
    }
}