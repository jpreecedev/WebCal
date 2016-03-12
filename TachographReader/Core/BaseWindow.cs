namespace TachographReader.Core
{
    using System.ComponentModel;
    using Fluent;
    using Library;
    using Shared.Helpers;

    public class BaseWindow : RibbonWindow
    {
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = MessageBoxHelper.AskQuestion(Properties.Resources.TXT_CONFIRM_EXIT) == false;

            if (!e.Cancel && MessageBoxHelper.AskQuestion(Properties.Resources.QTN_WOULD_YOU_LIKE_BACKUP_DATABASE))
            {
                BackupRestoreManager.BackUp();
            }
        }
    }
}