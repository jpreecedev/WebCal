namespace TachographReader.Shared.Workers
{
    public interface IPipeClient : IPipe
    {
        void SendMessage(string message);
        void Connect();
    }
}