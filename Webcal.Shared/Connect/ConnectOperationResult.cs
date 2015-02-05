namespace Webcal.Shared.Connect
{
    using System;

    public class ConnectOperationResult
    {
        public ConnectOperationResult()
        {
        }

        public ConnectOperationResult(string message)
        {
            Message = message;
        }

        public ConnectOperationResult(Exception exception)
        {
            Exception = exception;
        }

        public ConnectOperationResult(Exception exception, string message)
        {
            Exception = exception;
            Message = message;
        }

        public ConnectOperationResult(object data)
        {
            Data = data;
        }

        public object Data { get; set; }
        public Exception Exception { get; set; }
        public string Message { get; set; }

        public bool IsSuccess
        {
            get { return Exception == null; }
        }

        public bool HasData
        {
            get { return Data != null; }
        }

        public bool HasMessage
        {
            get { return !string.IsNullOrEmpty(Message); }
        }
    }
}