namespace Webcal.Shared.Connect
{
    public class FailureOperationResult : IConnectOperationResult
    {
        public FailureOperationResult()
        {
            
        }

        public FailureOperationResult(string message)
        {
            Message = message;
        }

        public bool Success
        {
            get { return false; }
        }

        public string Message { get; set; }
    }
}