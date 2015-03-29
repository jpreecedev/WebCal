namespace TachographReader.Shared.Workers
{
    public interface IPipeServer : IPipe
    {
        void Connect(IWorkerParameters parameters);
    }
}