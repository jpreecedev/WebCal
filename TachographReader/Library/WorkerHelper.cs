namespace TachographReader.Library
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using DataModel;
    using Properties;
    using Shared;
    using Shared.Workers;

    public static class WorkerHelper
    {
        private static readonly ICollection<IPipe> _workers;
        private static bool _isInitialized = false;
        private static Timer _timer;
        private static bool _isQueueProcessing;
        private static readonly object _syncLock = new object();

        static WorkerHelper()
        {
            _workers = new List<IPipe>();
        }

        public static void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _timer = new Timer(t => ProcessQueue(), null, 0, 60000);
        }

        public static void QueueTask(IWorkerTask workerTask)
        {
            using (TachographContext context = new TachographContext())
            {
                var task = (WorkerTask)workerTask;
                task.Added = DateTime.Now;

                context.WorkerTasks.Add(task);
                context.SaveChanges();
            }

            ProcessQueue();
        }

        public static void ProcessQueue()
        {
            lock (_syncLock)
            {
                if (_isQueueProcessing)
                {
                    return;
                }    
            }

            _isQueueProcessing = true;

            var task = new Task(() =>
            {
                using (var context = new TachographContext())
                {
                    foreach (var unprocessedTask in context.WorkerTasks.Where(workerTask => workerTask.IsProcessing == false && workerTask.Processed == null))
                    {
                        Type plugin = Find(unprocessedTask.TaskName);
                        if (plugin == null)
                        {
                            throw new InvalidOperationException(string.Format(Resources.EXEC_UNABLE_TO_FIND_PLUGIN, unprocessedTask.TaskName));
                        }

                        IPipeServer server = new PipeServer(plugin);
                        server.Completed += OnCompleted;
                        server.Error += OnError;

                        unprocessedTask.WorkerId = server.Id;
                        unprocessedTask.IsProcessing = true;

                        context.SaveChanges();

                        _workers.Add(server);

                        server.Connect(unprocessedTask.Parameters);
                    }
                }

                _isQueueProcessing = false;
            });

            task.Start();
        }

        private static void OnError(object sender, WorkerChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IPipe worker = _workers.FirstOrDefault(w => w.Id == e.WorkerId);

                if (worker == null)
                    return;

                try
                {
                    using (var context = new TachographContext())
                    {
                        var workerTaskEntity = context.WorkerTasks.FirstOrDefault(t => t.WorkerId == e.WorkerId);
                        if (workerTaskEntity != null)
                        {
                            workerTaskEntity.IsProcessing = false;
                            workerTaskEntity.Processed = null;
                            workerTaskEntity.Message = e.Message;
                        }

                        context.SaveChanges();
                    }
                }
                finally
                {
                    _workers.Remove(worker);
                }
            });
        }

        private static void OnCompleted(object sender, WorkerChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IPipe worker = _workers.FirstOrDefault(w => w.Id == e.WorkerId);

                if (worker == null)
                    return;

                try
                {
                    using (var context = new TachographContext())
                    {
                        var workerTaskEntity = context.WorkerTasks.FirstOrDefault(t => t.WorkerId == e.WorkerId);
                        if (workerTaskEntity != null)
                        {
                            workerTaskEntity.IsProcessing = false;
                            workerTaskEntity.Processed = DateTime.Now;
                            workerTaskEntity.Message = e.Message;
                        }

                        context.SaveChanges();
                    }
                }
                finally
                {
                    _workers.Remove(worker);
                }
            });
        }

        private static Type Find(WorkerTaskName name)
        {
            return name.GetAttribute<WorkerTaskPluginAttribute>().PluginType;
        }
    }
}