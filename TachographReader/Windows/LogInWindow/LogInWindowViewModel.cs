namespace TachographReader.Windows.LogInWindow
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Core;
    using DataModel;
    using DataModel.Library;
    using Library;
    using Properties;
    using Shared;
    using Shared.Helpers;

    public class LogInWindowViewModel : BaseModalWindowViewModel
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
            {
                return;
            }

            PasswordBox passwordBox = GetPasswordBox(window);
            if (passwordBox == null)
            {
                return;
            }

            var repository = GetInstance<IRepository<User>>();
            if (UserManagement.Validate(repository, Username, passwordBox.Password))
            {
                UserManagement.LoggedInUserName = Username;
                window.DialogResult = true;
                window.Close();
                return;
            }

            passwordBox.Clear();
            MessageBoxHelper.ShowError(Resources.ERR_CANNOT_VERIFY_USERNAME_PASSWORD, Window);
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