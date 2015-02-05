namespace Webcal.Shared.Connect
{
    using System;
    using Webcal.Connect.Shared;

    public interface IConnectClient : IDisposable
    {
        bool IsOpen { get; }

        void Open(IConnectKeys connectKeys);
        IConnectService Service { get; set; }

        void ForceClose();
        void Close();
    }
}