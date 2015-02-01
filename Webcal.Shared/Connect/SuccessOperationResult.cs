namespace Webcal.Shared.Connect
{
    public class SuccessOperationResult : IConnectOperationResult
    {
        public bool Success
        {
            get { return true; }
        }

        public string Message { get; set; }
    }
}