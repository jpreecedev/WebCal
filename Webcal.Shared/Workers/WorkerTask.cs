namespace Webcal.Shared.Workers
{
    public class WorkerTask : IWorkerTask
    {
        public WorkerTaskName TaskName { get; set; }

        public string[] Parameters { get; set; }

        public string GetParameters()
        {
            return string.Join(" ", Parameters);
        }
    }
}