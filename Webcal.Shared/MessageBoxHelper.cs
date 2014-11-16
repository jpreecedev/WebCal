namespace Webcal.Library
{
    using System.Windows;
    using Shared.Properties;
    using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

    public static class MessageBoxHelper
    {
        public static bool AskQuestion(string msg)
        {
            return MessageBox.Show(msg, Resources.TXT_QUESTION, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public static void ShowMessage(string msg)
        {
            MessageBox.Show(msg, Resources.TXT_INFORMATION, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowMessage(string msg, string caption)
        {
            MessageBox.Show(msg, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowError(string msg)
        {
            MessageBox.Show(msg, Resources.TXT_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowError(string msg, object[] parameters)
        {
            MessageBox.Show(string.Format(msg, parameters), Resources.TXT_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}