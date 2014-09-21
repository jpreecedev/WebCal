namespace Webcal.Shared.Workers
{
    using System;

    public interface IWorker : IDisposable
    {
        bool Started { get; }

        int Id { get; }
        void Start();
        void Stop();

        event EventHandler<WorkerChangedEventArgs> ProgressChanged;
        event EventHandler<WorkerChangedEventArgs> Completed;
    }
}