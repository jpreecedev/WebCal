namespace TachographReader.Shared.Connect
{
    using global::Connect.Shared;

    public interface IConnectClientCredentials
    {
        IConnectKeys ConnectKeys { get; }
    }
}