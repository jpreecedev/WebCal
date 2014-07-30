using System.Linq;
using System.Windows;
using System.Windows.Controls;
using StructureMap;
using Webcal.Core;
using Webcal.DataModel.Library;
using Webcal.DataModel.Repositories;
using Webcal.Library;
using Webcal.Properties;

namespace Webcal.Windows.LogInWindow
{
    public class LogInWindowViewModel : BaseNotification
    {
        #region Constructor

        public LogInWindowViewModel()
        {
            LogInCommand = new DelegateCommand<Window>(OnLogIn);
            CancelCommand = new DelegateCommand<Window>(OnCancel);
        }

        #endregion

        #region Public Properties

        public string Username { get; set; }

        #endregion

        #region Commands

        #region Command : Log In

        public DelegateCommand<Window> LogInCommand { get; set; }

        private void OnLogIn(Window window)
        {
            if (window == null)
                return;

            PasswordBox passwordBox = GetPasswordBox(window);
            if (passwordBox == null)
                return;

            var repository = ObjectFactory.GetInstance<UserRepository>();
            if (UserManagement.Validate(repository, Username, passwordBox.Password))
            {
                UserManagement.LoggedInUserName = Username;
                window.DialogResult = true;
                window.Close();
                return;
            }

            MessageBoxHelper.ShowError(Resources.ERR_CANNOT_VERIFY_USERNAME_PASSWORD);
        }

        #endregion

        #region Command : Cancel

        public DelegateCommand<Window> CancelCommand { get; set; }

        private static void OnCancel(Window window)
        {
            window.DialogResult = false;
            window.Close();
        }

        #endregion

        #endregion

        #region Private Methods

        private PasswordBox GetPasswordBox(DependencyObject window)
        {
            return window.FindVisualChildren<PasswordBox>().FirstOrDefault();
        }

        #endregion
    }
}
