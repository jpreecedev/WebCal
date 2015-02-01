namespace Webcal.Core
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Interop;
    using Windows;
    using Fluent;
    using Library;
    using Shared.Helpers;
    using Views;

    public class BaseWindow : RibbonWindow
    {
        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private const int DBT_DEVNODES_CHANGED = 0x0007;

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

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var source = PresentationSource.FromVisual(this) as HwndSource;

            if (source != null)
            {
                source.AddHook(WndProc);
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DEVICECHANGE && (wParam.ToInt32() == DBT_DEVICEARRIVAL || wParam.ToInt32() == DBT_DEVNODES_CHANGED) && wParam.ToInt32() != DBT_DEVICEREMOVECOMPLETE)
            {
                var newTachographViewModel = ((MainWindowViewModel) DataContext).View.DataContext as NewTachographViewModel;
                if (newTachographViewModel != null && newTachographViewModel.ReadFromCardCommand != null)
                {
                    newTachographViewModel.ReadFromCardCommand.Execute(new object());
                }
            }
            return IntPtr.Zero;
        }

        private bool IsNavigationLocked()
        {
            if (DataContext != null)
            {
                var context = DataContext as MainWindowViewModel;
                if (context != null)
                {
                    return context.IsNavigationLocked;
                }
            }

            return false;
        }
    }
}