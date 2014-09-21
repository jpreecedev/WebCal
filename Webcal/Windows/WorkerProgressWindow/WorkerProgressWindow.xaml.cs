namespace Webcal.Windows.WorkerProgressWindow
{
    using System.ComponentModel;
    using Core;

    public partial class WorkerProgressWindow : BaseModalWindow
    {
        public WorkerProgressWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            var dataContext = DataContext as BaseModalWindowViewModel;
            if (dataContext != null)
            {
                dataContext.OnClosing();
            }
        }
    }
}