namespace TachographReader.Windows.WorkerProgressWindow
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using Core;
    using DataModel.Core;
    using Library;
    using Properties;
    using Shared;
    using Shared.Helpers;
    using Shared.Workers;

    public class WorkerProgressWindowViewModel : BaseModalWindowViewModel
    {
        private Timer _timer;

        public WorkerProgressWindowViewModel()
        {
            Repository = ContainerBootstrapper.Resolve<IRepository<WorkerTask>>();
            RefreshRecentTasksCommand = new DelegateCommand<object>(OnRefreshRecentTasks);
            ReProcessTaskCommand = new DelegateCommand<Window>(OnReProcessTask);
            CloseCommand = new DelegateCommand<Window>(OnClose);

            RefreshRecentTasksCommand.Execute(null);

            _timer = new Timer(t => Application.Current.Dispatcher.Invoke(() =>
            {
                RefreshRecentTasksCommand.Execute(null);
            }), null, 30000, 30000);
        }

        public IRepository<WorkerTask> Repository { get; set; }

        public ObservableCollection<WorkerTask> RecentTasks { get; set; }

        public WorkerTask SelectedTask { get; set; }

        public DelegateCommand<object> RefreshRecentTasksCommand { get; set; }

        public DelegateCommand<Window> ReProcessTaskCommand { get; set; }

        public DelegateCommand<Window> CloseCommand { get; set; }

        public override void OnClosing()
        {
            _timer.Dispose();
            _timer = null;
            base.OnClosing();
        }

        private void OnRefreshRecentTasks(object parameter)
        {
            if (RecentTasks == null)
            {
                RecentTasks = new ObservableCollection<WorkerTask>();
            }

            RecentTasks.Clear();
            RecentTasks.AddRange(Repository.Where(t => t.Deleted == null && t.Added >= DateTime.Now.AddHours(-48)).OrderByDescending(t => t.Added));
        }

        private void OnClose(Window window)
        {
            if (window == null)
            {
                return;
            }

            window.Close();
        }

        private void OnReProcessTask(Window window)
        {
            if (SelectedTask == null)
            {
                return;
            }

            var workerTask = Repository.FirstOrDefault(t => t.WorkerId == SelectedTask.WorkerId);
            if (workerTask != null)
            {
                workerTask.IsProcessing = false;
                workerTask.Deleted = null;
                workerTask.Processed = null;

                Repository.AddOrUpdate(workerTask);

                MessageBoxHelper.ShowMessage(Resources.TXT_WORKER_PROGRESS_RE_PROCESSED_SOON, window);
                RefreshRecentTasksCommand.Execute(null);
            }
        }
    }
}