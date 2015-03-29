namespace TachographReader.Shared.Workers
{
    using System;

    public class WorkerChangedEventArgs : EventArgs
    {
        public Guid WorkerId { get; set; }
        public string Message { get; set; }
        public WorkerTaskName TaskName { get; set; }
    }
}