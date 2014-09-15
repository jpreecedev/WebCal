namespace Webcal.Windows.LogInWindow
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Core;
    using DataModel.Core;
    using DataModel.Library;
    using DataModel.Repositories;
    using Library;
    using Properties;

    public class LogInWindowViewModel : BaseNotification
    {
        public LogInWindowViewModel()
        {
            LogInCommand = new DelegateCommand<Window>(OnLogIn);
            CancelCommand = new DelegateCommand<Window>(OnCancel);

#if DEBUG
            Username = "superuser";
#endif
        }

        public string Username { get; set; }
        
        public DelegateCommand<Window> LogInCommand { get; set; }
        
        public DelegateCommand<Window> CancelCommand { get; set; }

        private void OnLogIn(Window window)
        {
            if (window == null)
                return;

            PasswordBox passwordBox = GetPasswordBox(window);
            if (passwordBox == null)
                return;

            var repository = ContainerBootstrapper.Container.GetInstance<UserRepository>();
            if (UserManagement.Validate(repository, Username, passwordBox.Password))
            {
                UserManagement.LoggedInUserName = Username;
                window.DialogResult = true;
                window.Close();
                return;
            }

            MessageBoxHelper.ShowError(Resources.ERR_CANNOT_VERIFY_USERNAME_PASSWORD);
        }

        private static void OnCancel(Window window)
        {
            window.DialogResult = false;
            window.Close();
        }
        
        private PasswordBox GetPasswordBox(DependencyObject window)
        {
            return window.FindVisualChildren<PasswordBox>().FirstOrDefault();
        }
    }
}