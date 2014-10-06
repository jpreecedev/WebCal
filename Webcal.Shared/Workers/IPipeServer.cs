namespace Webcal.Shared.Workers
{
    public interface IPipeServer : IPipe
    {
        void Connect(IWorkerParameters parameters);
    }
}