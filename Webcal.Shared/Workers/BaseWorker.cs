namespace Webcal.Shared.Workers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Pipes;
    using System.Threading.Tasks;

    public class BaseWorker : IDisposable
    {
        private Process _client;
        private Boolean _disposed;
        private AnonymousPipeServerStream _server;

        public int Id
        {
            get
            {
                if (_client == null)
                {
                    return 0;
                }

                return _client.Id;
            }
        }

        public bool Started { get; private set; }

        public void Dispose()
        {
            Dispose(true);

            if (Started)
            {
                Stop();
            }

            GC.SuppressFinalize(this);
        }

        ~BaseWorker()
        {
            Dispose(false);
        }

        public event EventHandler<WorkerChangedEventArgs> ProgressChanged;

        public void Client(string path)
        {
            _client = new Process
            {
                StartInfo =
                {
                    FileName = path,
                    UseShellExecute = false,
                    Arguments = _server.GetClientHandleAsString()
                }
            };

            _client.Start();

            _server.DisposeLocalCopyOfClientHandle();
        }

        public void Stop()
        {
            Started = false;
            _server.Dispose();
        }

        protected void Close()
        {
            if (_client == null)
            {
                return;
            }

            _client.Kill();
            _client.Close();
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_client != null)
                {
                    _client.Dispose();
                }
                if (_server != null)
                {
                    _server.Dispose();
                }
            }

            _disposed = true;
        }

        protected virtual void OnProgressChanged(KeyValuePair<int, string>? message)
        {
            if (message == null)
            {
                return;
            }

            EventHandler<WorkerChangedEventArgs> handler = ProgressChanged;
            if (handler != null)
            {
                handler(this, new WorkerChangedEventArgs { WorkerId = message.Value.Key, Message = message.Value.Value });
            }
        }

        protected void StartServer()
        {
            if (Started)
            {
                return;
            }

            _server = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);

            var task = new Task(() =>
            {
                Started = true;

                using (var sr = new StreamReader(_server))
                {
                    string temp;

                    while ((temp = sr.ReadLine()) != null)
                    {
                        OnProgressChanged(MessageParser.Parse(temp));
                    }
                }
            });

            task.Start();
        }
    }
}