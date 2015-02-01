namespace Webcal.Shared.Connect
{
    public interface IConnectKeys
    {
        string Url { get; set; }

        int LicenseKey { get; set; }
        string CompanyKey { get; set; }
        string MachineKey { get; set; }
    }
}