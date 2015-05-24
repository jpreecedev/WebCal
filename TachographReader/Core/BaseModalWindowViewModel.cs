namespace TachographReader.Core
{
    using System;
    using System.Windows;
    using System.Windows.Threading;
    using Windows.ProgressWindow;
    using Connect.Shared.Models;
    using DataModel.Core;
    using Properties;

    public class BaseModalWindowViewModel : BaseNotification
    {
        private ProgressWindow _progressWindow;

        public Window Window { get; set; }

        public TService GetInstance<TService>()
        {
            return ContainerBootstrapper.Resolve<TService>();
        }

        public virtual void OnClosing()
        {
        }

        protected virtual void ShowProgressWindow()
        {
            _progressWindow = new ProgressWindow();
            ((ProgressWindowViewModel)_progressWindow.DataContext).ProgressText = Resources.TXT_LOADING;
            _progressWindow.ShowDialog();
        }

        protected virtual void SetProgressWindowMessage(string message)
        {
            if (_progressWindow == null)
            {
                return;
            }

            ((ProgressWindowViewModel)_progressWindow.DataContext).ProgressText = message;
        }

        protected virtual void CloseProgressWindow()
        {
            if (_progressWindow != null)
            {
                _progressWindow.Close();
                _progressWindow = null;
            }
        }
    }
}