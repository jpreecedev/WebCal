namespace Webcal.Library.ViewModels
{
    using System;
    using Shared.Workers;

    public class WorkerViewModel
    {
        public Guid WorkerId { get; set; }
        public string Message { get; set; }
        public WorkerTaskName TaskName { get; set; }
    }
}