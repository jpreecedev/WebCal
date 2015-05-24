namespace TachographReader.Core
{
    using System;
    using System.ComponentModel;
    using System.Windows;

    public class BaseModalWindow : Window
    {
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            var dataContext = DataContext as BaseModalWindowViewModel;
            if (dataContext != null)
            {
                dataContext.OnClosing();
            }
        }

        public new bool? ShowDialog()
        {
            var dataContext = DataContext as BaseModalWindowViewModel;
            if (dataContext != null)
            {
                if (!Equals(this, Application.Current.MainWindow))
                {
                    Owner = dataContext.Window = Application.Current.MainWindow;
                }
            }
            try
            {
                return base.ShowDialog();

            }
            catch (Exception)
            {
                
            }

            return null;
        }
    }
}