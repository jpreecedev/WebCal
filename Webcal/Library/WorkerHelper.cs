namespace Webcal.Library
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Properties;
    using Shared;
    using Shared.Workers;
    using ViewModels;

    public static class WorkerHelper
    {
        private static readonly ICollection<IPipe> _workers;
        private static ObservableCollection<WorkerViewModel> _workerTasks;

        static WorkerHelper()
        {
            _workers = new List<IPipe>();
        }

        public static void RunTask(IWorkerTask workerTask)
        {
            var task = new Task(() =>
            {
                Plugin plugin = Find(workerTask.TaskName);
                if (plugin == null)
                {
                    throw new InvalidOperationException(string.Format(Resources.EXEC_UNABLE_TO_FIND_PLUGIN, workerTask.TaskName));
                }

                IPipeServer server = new PipeServer(plugin.Path);
                server.ProgressChanged += OnProgressChanged;
                server.Completed += OnCompleted;

                _workers.Add(server);
                server.Connect(workerTask.Parameters);
            });

            task.Start();
        }

        public static void StopTracking()
        {
            _workerTasks = null;
        }

        public static void Track(ObservableCollection<WorkerViewModel> workerTasks)
        {
            _workerTasks = workerTasks;
        }

        private static void OnProgressChanged(object sender, WorkerChangedEventArgs e)
        {
            if (_workerTasks == null)
            {
                return;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                WorkerViewModel workerTask = _workerTasks.FirstOrDefault(c => c.WorkerId == e.WorkerId);
                if (workerTask != null)
                {
                    workerTask.Message = e.Message;
                }
                else
                {
                    var workerViewModel = new WorkerViewModel
                    {
                        Message = e.Message,
                        WorkerId = e.WorkerId
                    };

                    _workerTasks.Add(workerViewModel);
                }
            });
        }

        private static void OnCompleted(object sender, WorkerChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IPipe worker = _workers.FirstOrDefault(w => w.Id == e.WorkerId);

                if (worker != null)
                {
                    worker.Dispose();
                    _workers.Remove(worker);

                    if (_workerTasks != null && _workerTasks.Count > 0)
                    {
                        WorkerViewModel workerTask = _workerTasks.FirstOrDefault(task => task.WorkerId == worker.Id);
                        if (workerTask != null)
                        {
                            _workerTasks.Remove(workerTask);
                        }
                    }
                }
            });
        }

        private static Plugin Find(WorkerTaskName name)
        {
            foreach (Plugin plugin in WebcalConfigurationSection.Instance.PluginsCollection)
            {
                if (string.Equals(string.Format("{0}Worker", name), plugin.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    return plugin;
                }
            }

            return null;
        }
    }
}