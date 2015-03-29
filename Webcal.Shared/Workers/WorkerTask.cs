namespace TachographReader.Shared.Workers
{
    public class WorkerTask : IWorkerTask
    {
        public WorkerTaskName TaskName { get; set; }
        public IWorkerParameters Parameters { get; set; }
    }
}