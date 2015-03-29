namespace TachographReader.Windows.WorkerProgressWindow
{
    using System.Collections.ObjectModel;
    using System.Windows;
    using Core;
    using Library;
    using Library.ViewModels;

    public class WorkerProgressWindowViewModel : BaseModalWindowViewModel
    {
        public WorkerProgressWindowViewModel()
        {
            Tasks = new ObservableCollection<WorkerViewModel>();
            WorkerHelper.Track(Tasks);

            CloseCommand = new DelegateCommand<Window>(OnClose);
        }

        public ObservableCollection<WorkerViewModel> Tasks { get; set; }
        public DelegateCommand<Window> CloseCommand { get; set; }

        public override void OnClosing()
        {
            WorkerHelper.StopTracking();
        }

        private void OnClose(Window window)
        {
            if (window == null)
            {
                return;
            }

            window.Close();
        }
    }
}