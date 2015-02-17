namespace Webcal.ConnectClient
{
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.ServiceModel;
    using System.ServiceModel.Security.Tokens;
    using Connect.Shared;

    public class ConnectClientCredentialsSecurityTokenManager : ClientCredentialsSecurityTokenManager
    {
        private readonly ConnectClientCredentials _credentials;

        public ConnectClientCredentialsSecurityTokenManager(ConnectClientCredentials connectClientCredentials)
            : base(connectClientCredentials)
        {
            _credentials = connectClientCredentials;
        }

        public override SecurityTokenProvider CreateSecurityTokenProvider(SecurityTokenRequirement tokenRequirement)
        {
            if (tokenRequirement.TokenType == ConnectConstants.ConnectTokenType)
            {
                // Handle this token for Custom.
                return new ConnectTokenProvider(_credentials.ConnectKeys);
            }
            if (tokenRequirement is InitiatorServiceModelSecurityTokenRequirement)
            {
                // Return server certificate.
                if (tokenRequirement.TokenType == SecurityTokenTypes.X509Certificate)
                {
                    return new X509SecurityTokenProvider(_credentials.ServiceCertificate.DefaultCertificate);
                }
            }
            return base.CreateSecurityTokenProvider(tokenRequirement);
        }

        public override SecurityTokenSerializer CreateSecurityTokenSerializer(SecurityTokenVersion version)
        {
            return new ConnectSecurityTokenSerializer(version);
        }
    }
}