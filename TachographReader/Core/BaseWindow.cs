namespace TachographReader.Core
{
    using System.ComponentModel;
    using Windows;
    using Fluent;
    using Library;
    using Shared.Helpers;

    public class BaseWindow : RibbonWindow
    {
        protected override void OnClosing(CancelEventArgs e)
        {
            if (IsNavigationLocked())
            {
                e.Cancel = true;
                return;
            }

            e.Cancel = MessageBoxHelper.AskQuestion(Properties.Resources.TXT_CONFIRM_EXIT) == false;

            if (!e.Cancel && MessageBoxHelper.AskQuestion(Properties.Resources.QTN_WOULD_YOU_LIKE_BACKUP_DATABASE))
            {
                BackupRestoreManager.BackUp();
            }
        }

        private bool IsNavigationLocked()
        {
            var context = DataContext as MainWindowViewModel;
            if (context != null)
            {
                return context.IsNavigationLocked;
            }

            return false;
        }
    }
}