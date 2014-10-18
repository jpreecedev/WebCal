namespace Webcal.Shared.Workers
{
    using System;

    public interface IWorker : IDisposable
    {
        bool Started { get; }
        Guid Id { get; }
        void Start(IWorkerParameters parameters);
        event EventHandler<WorkerChangedEventArgs> ProgressChanged;
        event EventHandler<WorkerChangedEventArgs> Completed;
    }
}