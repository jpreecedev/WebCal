namespace TachographReader.Shared.Connect
{
    using System;
    using global::Connect.Shared;

    public interface IConnectClient : IDisposable
    {
        bool IsOpen { get; }

        void Open(IConnectKeys connectKeys);
        IConnectService Service { get; set; }

        void ForceClose();
        void Close();
    }
}