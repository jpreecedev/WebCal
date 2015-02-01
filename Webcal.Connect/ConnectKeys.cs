namespace Webcal.Connect
{
    using System;
    using Shared.Connect;

    [Serializable]
    public class ConnectKeys : IConnectKeys
    {
        public ConnectKeys(string url, int licenseKey, string companyKey, string machineKey)
        {
            Url = url;
            LicenseKey = licenseKey;
            CompanyKey = companyKey;
            MachineKey = machineKey;
        }

        public string Url { get; set; }
        public int LicenseKey { get; set; }
        public string CompanyKey { get; set; }
        public string MachineKey { get; set; }
    }
}