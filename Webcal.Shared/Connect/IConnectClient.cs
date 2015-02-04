namespace Webcal.Shared.Connect
{
    using System;
    using Webcal.Connect.Shared;

    public interface IConnectClient : IDisposable
    {
        bool IsOpen { get; }

        IConnectOperationResult Open(IConnectKeys connectKeys);
        IConnectService Service { get; set; }

        void CallAsync<TResult>(Func<IConnectClient, TResult> beginCall, Action<TResult> endCall, Action<Exception> exceptionHandler);

        void ForceClose();
        void Close();
    }
}