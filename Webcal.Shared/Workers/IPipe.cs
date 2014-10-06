namespace Webcal.Shared.Workers
{
    using System;

    public interface IPipe : IDisposable
    {
        Guid Id { get; }

        void Close();

        EventHandler<WorkerChangedEventArgs> ProgressChanged { get; set; }
        EventHandler<WorkerChangedEventArgs> Completed { get; set; }
    }
}