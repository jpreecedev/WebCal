namespace Webcal.Library
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Shared.Workers;
    using ViewModels;

    public static class WorkerHelper
    {
        private static readonly ICollection<IWorker> _workers;
        private static ObservableCollection<WorkerViewModel> _workerTasks;

        static WorkerHelper()
        {
            _workers = new List<IWorker>();
        }

        public static void RunTask(IWorkerTask workerTask)
        {
            var worker = PluginHelper.Load(string.Format("{0}Worker", workerTask.TaskName));
            worker.ProgressChanged += OnProgressChanged;
            worker.Completed += OnCompleted;
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

            var workerTask = _workerTasks.FirstOrDefault(c => c.WorkerId == e.WorkerId);
            if (workerTask != null)
            {
                workerTask.Message = e.Message;
            }
            else
            {
                var workerViewModel = new WorkerViewModel
                {
                    Message = e.Message,
                    WorkerId = e.WorkerId,
                };

                _workerTasks.Add(workerViewModel);
            }
        }

        private static void OnCompleted(object sender, WorkerChangedEventArgs e)
        {
            var worker = _workers.FirstOrDefault(w => w.Id == e.WorkerId);

            if (worker != null)
            {
                worker.Dispose();
                _workers.Remove(worker);
            }
        }
    }
}