namespace TachographReader.Shared.Workers
{
    using System.Linq;

    public class BaseWorkerEntryPoint
    {
        protected static IWorkerParameters ProcessParameters(string[] args)
        {
            IWorkerParameters result = new WorkerParameters();
            result.Deserialize(string.Join(" ", args.Where((s, i) => i > 0)));
            return result;
        }
    }
}