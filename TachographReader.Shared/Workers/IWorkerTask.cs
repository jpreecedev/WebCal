namespace TachographReader.Shared.Workers
{
    public interface IWorkerTask
    {
        WorkerTaskName TaskName { get; set; }
        IWorkerParameters GetWorkerParameters();
        void SetWorkerParameters(IWorkerParameters parameters);
    }
}