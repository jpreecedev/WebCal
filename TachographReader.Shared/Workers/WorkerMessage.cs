namespace TachographReader.Shared.Workers
{
    using System;

    public class WorkerMessage
    {
        public Guid WorkerId { get; set; }
        public string Message { get; set; }
    }
}