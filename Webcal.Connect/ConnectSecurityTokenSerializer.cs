namespace Webcal.Connect
{
    using System;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.ServiceModel.Security;
    using System.Xml;
    using Shared;
    using Shared.Connect;

    public class ConnectSecurityTokenSerializer : WSSecurityTokenSerializer
    {
        private readonly SecurityTokenVersion _version;

        public ConnectSecurityTokenSerializer(SecurityTokenVersion version)
        {
            _version = version;
        }

        protected override bool CanReadTokenCore(XmlReader reader)
        {
            XmlDictionaryReader localReader = XmlDictionaryReader.CreateDictionaryReader(reader);
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.IsStartElement(Constants.ConnectTokenName, Constants.ConnectNamespace))
            {
                return true;
            }
            return base.CanReadTokenCore(reader);
        }

        protected override SecurityToken ReadTokenCore(XmlReader reader, SecurityTokenResolver tokenResolver)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.IsStartElement(Constants.ConnectTokenName, Constants.ConnectNamespace))
            {
                string id = reader.GetAttribute(Constants.Id, Constants.WsUtilityNamespace);

                reader.ReadStartElement();

                string licenseKey = reader.ReadElementString(Constants.ConnectLicenseKeyElementName, Constants.ConnectNamespace);
                string companyKey = reader.ReadElementString(Constants.ConnectCompanyKeyElementName, Constants.ConnectNamespace);
                string machineKey = reader.ReadElementString(Constants.ConnectMachineKeyElementName, Constants.ConnectNamespace);

                reader.ReadEndElement();

                var cardInfo = new ConnectKeys(GetUrl(), int.Parse(licenseKey), companyKey, machineKey);

                return new ConnectToken(cardInfo, id);
            }
            return DefaultInstance.ReadToken(reader, tokenResolver);
        }

        private static string GetUrl()
        {
            foreach (ConnectElement element in WebcalConfigurationSection.Instance.ConnectCollection)
            {
                return element.Address;
            }

            return string.Empty;
        }

        protected override bool CanWriteTokenCore(SecurityToken token)
        {
            if (token is ConnectToken)
            {
                return true;
            }
            return base.CanWriteTokenCore(token);
        }

        protected override void WriteTokenCore(XmlWriter writer, SecurityToken token)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }

            var c = token as ConnectToken;
            if (c != null)
            {
                writer.WriteStartElement(Constants.ConnectTokenPrefix, Constants.ConnectTokenName, Constants.ConnectNamespace);
                writer.WriteAttributeString(Constants.WsUtilityPrefix, Constants.Id, Constants.WsUtilityNamespace, token.Id);
                writer.WriteElementString(Constants.ConnectLicenseKeyElementName, Constants.ConnectNamespace, XmlConvert.ToString(c.ConnectKeys.LicenseKey));
                writer.WriteElementString(Constants.ConnectCompanyKeyElementName, Constants.ConnectNamespace, c.ConnectKeys.CompanyKey);
                writer.WriteElementString(Constants.ConnectMachineKeyElementName, Constants.ConnectNamespace, c.ConnectKeys.MachineKey);
                writer.WriteEndElement();
                writer.Flush();
            }
            else
            {
                base.WriteTokenCore(writer, token);
            }
        }
    }
}