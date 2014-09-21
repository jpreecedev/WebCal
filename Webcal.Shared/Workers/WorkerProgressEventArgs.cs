namespace Webcal.Shared.Workers
{
    using System;

    public class WorkerChangedEventArgs : EventArgs
    {
        public int WorkerId { get; set; }

        public string Message { get; set; }
    }
}