namespace Webcal.Shared.Workers
{
    public interface IPipeClient : IPipe
    {
        void SendMessage(string message);
        void Connect();
    }
}