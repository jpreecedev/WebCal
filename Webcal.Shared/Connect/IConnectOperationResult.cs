namespace Webcal.Shared.Connect
{
    public interface IConnectOperationResult
    {
        bool Success { get; }
        string Message { get; set; }
    }
}