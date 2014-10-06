namespace Webcal.Shared.Workers
{
    using System.Diagnostics;
    using System.IO;
    using System.IO.Pipes;

    public class PipeServer : BasePipeProvider, IPipeServer
    {
        private readonly Process _pipeClient = new Process();
        private readonly AnonymousPipeServerStream _pipeServer = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
        private StreamReader _reader;

        public PipeServer(string clientPath)
        {
            _pipeClient.StartInfo.FileName = clientPath;
        }

        public void Connect(IWorkerParameters parameters)
        {
            _pipeClient.StartInfo.Arguments = _pipeServer.GetClientHandleAsString() + " " + parameters.Serialize().DoubleEscape();
            _pipeClient.StartInfo.UseShellExecute = false;
            _pipeClient.StartInfo.CreateNoWindow = true;
            _pipeClient.Start();

            _pipeServer.DisposeLocalCopyOfClientHandle();

            _reader = new StreamReader(_pipeServer);

            string temp;
            while ((temp = _reader.ReadLine()) != null)
            {
                OnChanged(ProgressChanged, temp);
            }

            OnChanged(Completed, string.Empty);
        }

        public void Close()
        {
            _pipeClient.WaitForExit();
            _pipeClient.Close();
        }

        public void Dispose()
        {
            Close();

            if (_pipeClient != null)
            {
                _pipeClient.Dispose();
            }
            if (_pipeServer != null)
            {
                _pipeServer.Dispose();
            }
            if (_reader != null)
            {
                _reader.Dispose();
            }
        }
    }
}