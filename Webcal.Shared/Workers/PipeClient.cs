namespace Webcal.Shared.Workers
{
    using System.IO;
    using System.IO.Pipes;

    public class PipeClient : BasePipeProvider, IPipeClient
    {
        private readonly PipeStream _pipeClient;
        private StreamWriter _writer;

        public PipeClient(string handle)
        {
            _pipeClient = new AnonymousPipeClientStream(PipeDirection.Out, handle);
        }
        
        public void Connect()
        {
            _writer = new StreamWriter(_pipeClient)
            {
                AutoFlush = true
            };
        }

        public void Close()
        {
            _writer.Close();
            _pipeClient.Close();
        }
        
        public void SendMessage(string message)
        {
            if (_writer != null)
            {
                _writer.WriteLine(message);
                _pipeClient.WaitForPipeDrain();
            }
        }

        public void Dispose()
        {
            Close();

            if (_writer != null)
            {
                _writer.Dispose();
            }
            if (_pipeClient != null)
            {
                _pipeClient.Dispose();
            }
        }
    }
}