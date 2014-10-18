namespace Webcal.Shared.Workers
{
    using System;

    public class BaseWorker : IWorker
    {
        public BaseWorker(Action<string> sendMessage)
        {
            SendMessage = sendMessage;
            Id = Guid.NewGuid();
        }

        protected Action<string> SendMessage { get; set; }
        public bool Started { get; protected set; }
        public Guid Id { get; private set; }

        public virtual void Start(IWorkerParameters parameters)
        {
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public event EventHandler<WorkerChangedEventArgs> ProgressChanged;
        public event EventHandler<WorkerChangedEventArgs> Completed;

        protected virtual void OnProgressChanged(WorkerMessage message)
        {
            if (message == null)
            {
                return;
            }

            EventHandler<WorkerChangedEventArgs> handler = ProgressChanged;
            if (handler != null)
            {
                handler(this, new WorkerChangedEventArgs
                {
                    Message = message.Message,
                    WorkerId = message.WorkerId
                });
            }
        }

        protected virtual void OnCompleted(WorkerChangedEventArgs e)
        {
            EventHandler<WorkerChangedEventArgs> handler = Completed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}