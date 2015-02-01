namespace Webcal.Connect
{
    using System;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.ServiceModel.Security.Tokens;

    public class ConnectTokenParameters : SecurityTokenParameters
    {
        public ConnectTokenParameters()
        {
        }

        protected ConnectTokenParameters(ConnectTokenParameters other)
            : base(other)
        {
        }

        protected override bool HasAsymmetricKey
        {
            get { return false; }
        }

        protected override bool SupportsClientAuthentication
        {
            get { return true; }
        }

        protected override bool SupportsClientWindowsIdentity
        {
            get { return false; }
        }

        protected override bool SupportsServerAuthentication
        {
            get { return false; }
        }

        protected override SecurityTokenParameters CloneCore()
        {
            return new ConnectTokenParameters(this);
        }

        protected override void InitializeSecurityTokenRequirement(SecurityTokenRequirement requirement)
        {
            requirement.TokenType = Constants.ConnectTokenType;
        }

        protected override SecurityKeyIdentifierClause CreateKeyIdentifierClause(SecurityToken token, SecurityTokenReferenceStyle referenceStyle)
        {
            if (referenceStyle == SecurityTokenReferenceStyle.Internal)
            {
                return token.CreateKeyIdentifierClause<LocalIdKeyIdentifierClause>();
            }
            throw new NotSupportedException("External references are not supported for connect tokens");
        }
    }
}