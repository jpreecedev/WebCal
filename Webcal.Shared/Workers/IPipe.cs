namespace Webcal.Shared.Workers
{
    using System;

    public interface IPipe : IDisposable
    {
        Guid Id { get; }
        EventHandler<WorkerChangedEventArgs> ProgressChanged { get; set; }
        EventHandler<WorkerChangedEventArgs> Completed { get; set; }
        void Close();
    }
}