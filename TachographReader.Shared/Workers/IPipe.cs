namespace TachographReader.Shared.Workers
{
    using System;

    public interface IPipe
    {
        Guid Id { get; }
        EventHandler<WorkerChangedEventArgs> Completed { get; set; }
        EventHandler<WorkerChangedEventArgs> Error { get; set; }
    }
}