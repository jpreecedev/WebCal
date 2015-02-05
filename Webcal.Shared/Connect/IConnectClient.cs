namespace Webcal.Shared.Connect
{
    using System;
    using Webcal.Connect.Shared;

    public interface IConnectClient : IAsyncClient, IDisposable
    {
        bool IsOpen { get; }

        IConnectOperationResult Open(IConnectKeys connectKeys);
        IConnectService Service { get; set; }

        void ForceClose();
        void Close();
    }
}