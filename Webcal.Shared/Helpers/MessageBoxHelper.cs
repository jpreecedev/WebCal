namespace Webcal.Shared.Helpers
{
    using System;
    using System.Windows;
    using System.Windows.Threading;
    using Shared.Properties;
    using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

    public static class MessageBoxHelper
    {
        public static bool AskQuestion(string msg, Window owner = null)
        {
            if (owner == null)
            {
                owner = Application.Current.MainWindow;
            }

            return MessageBox.Show(owner, msg, Resources.TXT_QUESTION, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public static void ShowMessage(string msg, Window owner = null)
        {
            Show(owner, msg, Resources.TXT_INFORMATION, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowMessage(string msg, string caption, Window owner = null)
        {
            Show(owner, msg, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowError(string msg, Window owner = null)
        {
            Show(owner, msg, Resources.TXT_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowError(string msg, object[] parameters, Window owner = null)
        {
            Show(owner, string.Format(msg, parameters), Resources.TXT_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static void Show(Window owner, string message, string caption, MessageBoxButton buttons, MessageBoxImage messageBoxImage)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (owner == null)
                {
                    owner = Application.Current.MainWindow;
                }
                MessageBox.Show(owner, message, caption, buttons, messageBoxImage);

            }, DispatcherPriority.Normal);
        }
    }
}