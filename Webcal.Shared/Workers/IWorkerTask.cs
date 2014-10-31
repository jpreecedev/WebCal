﻿namespace Webcal.Shared.Workers
{
    public interface IWorkerTask
    {
        WorkerTaskName TaskName { get; set; }
        IWorkerParameters Parameters { get; set; }
    }
}