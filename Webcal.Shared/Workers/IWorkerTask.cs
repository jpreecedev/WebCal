namespace Webcal.Shared.Workers
{
    public interface IWorkerTask
    {
        WorkerTaskName TaskName { get; set; }
        string[] Parameters { get; set; }
    }
}