namespace TachographReader.Shared.Workers
{
    using System;

    public class BaseWorker : IWorker
    {
        public BaseWorker()
        {
            Id = Guid.NewGuid();
        }

        public bool Started { get; protected set; }
        public Guid Id { get; private set; }

        public virtual void Start(IWorkerParameters parameters)
        {
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public event EventHandler<WorkerChangedEventArgs> Completed;
        
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