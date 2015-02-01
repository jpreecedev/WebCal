namespace Webcal.Shared.Connect
{
    using System;

    public interface IConnectClient : IDisposable
    {
        IConnectOperationResult Open(IConnectKeys connectKeys);

        void ForceClose();
        void Close();
    }
}