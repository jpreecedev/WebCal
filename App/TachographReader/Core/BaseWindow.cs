using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using Fluent;
using Webcal.Library;
using Webcal.Views;
using Webcal.Windows;

namespace Webcal.Core
{
    public class BaseWindow : RibbonWindow
    {
        const int WM_DEVICECHANGE = 0x0219;
        const int DBT_DEVICEARRIVAL = 0x8000;
        const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        const int DBT_DEVNODES_CHANGED = 0x0007;

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
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;

            if (source != null)
            {
                source.AddHook(WndProc);
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
                if (msg == WM_DEVICECHANGE && (wParam.ToInt32() == DBT_DEVICEARRIVAL || wParam.ToInt32() == DBT_DEVNODES_CHANGED) && wParam.ToInt32() != DBT_DEVICEREMOVECOMPLETE)
                    {
                        NewTachographViewModel newTachographViewModel = ((MainWindowViewModel)DataContext).View.DataContext as NewTachographViewModel;
                        if (newTachographViewModel != null && newTachographViewModel.ReadFromCardCommand != null)
                            newTachographViewModel.ReadFromCardCommand.Execute(new object());
                }
            return IntPtr.Zero;
        }

        private bool IsNavigationLocked()
        {
            if (DataContext != null)
            {
                MainWindowViewModel context = DataContext as MainWindowViewModel;
                if (context != null)
                    return context.IsNavigationLocked;
            }

            return false;
        }
    }
}
