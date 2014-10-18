namespace Webcal.Shared.Workers
{
    using System;

    public class BasePipeProvider
    {
        public BasePipeProvider()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; protected set; }
        public EventHandler<WorkerChangedEventArgs> ProgressChanged { get; set; }
        public EventHandler<WorkerChangedEventArgs> Completed { get; set; }

        protected void OnChanged(EventHandler<WorkerChangedEventArgs> handler, string message)
        {
            var changedHandler = handler;
            if (changedHandler != null)
            {
                changedHandler.Invoke(this, new WorkerChangedEventArgs
                {
                    Message = message,
                    WorkerId = Id
                });
            }
        }
    }
}