namespace Webcal.Connect
{
    public class Constants
    {
        public const string ConnectNamespace = "https://webcal-connect/";

        public const string ConnectLicenseKeyClaim = ConnectNamespace + "Claims/LicenseKey";
        public const string ConnectMachineKeyClaim = ConnectNamespace + "Claims/MachineKey";
        public const string ConnectCompanyKeyClaim = ConnectNamespace + "Claims/CompanyKey";
        public const string ConnectTokenType = ConnectNamespace + "Tokens/ConnectToken";

        internal const string ConnectTokenPrefix = "ct";
        internal const string ConnectTokenName = "ConnectToken";
        internal const string Id = "Id";
        internal const string WsUtilityPrefix = "wsu";
        internal const string WsUtilityNamespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";

        internal const string ConnectLicenseKeyElementName = "LicenseKey";
        internal const string ConnectMachineKeyElementName = "MachineKey";
        internal const string ConnectCompanyKeyElementName = "CompanyKey";
    }
}