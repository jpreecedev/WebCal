namespace Webcal.Shared.Connect
{
    using Webcal.Connect.Shared;

    public interface IConnectClientCredentials
    {
        IConnectKeys ConnectKeys { get; }
    }
}