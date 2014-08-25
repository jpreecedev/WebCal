namespace Webcal.Views.Settings
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using Windows.SignatureCaptureWindow;
    using Core;
    using DataModel;
    using DataModel.Library;
    using Library;
    using Properties;
    using Shared;
    using StructureMap;

    public class UserManagementViewModel : BaseSettingsViewModel
    {
        public string NewUserName { get; set; }

        public IRepository<User> Repository { get; set; }

        public ObservableCollection<User> Users { get; set; }

        public User SelectedUser
        {
            get { return UserManagement.SelectedUser; }
            set { UserManagement.SelectedUser = value; }
        }

        public DelegateCommand<PasswordBox> AddCommand { get; set; }
        public DelegateCommand<PasswordBox> ClearCommand { get; set; }
        public DelegateCommand<Grid> ChangePasswordCommand { get; set; }
        public DelegateCommand<Grid> ResetPasswordCommand { get; set; }
        public DelegateCommand<Grid> ClearPasswordCommand { get; set; }
        public DelegateCommand<object> ManageSignaturesCommand { get; set; }
        
        protected override void InitialiseCommands()
        {
            AddCommand = new DelegateCommand<PasswordBox>(OnAdd);
            ClearCommand = new DelegateCommand<PasswordBox>(OnClear);
            ChangePasswordCommand = new DelegateCommand<Grid>(OnChangePassword);
            ResetPasswordCommand = new DelegateCommand<Grid>(OnResetPassword);
            ClearPasswordCommand = new DelegateCommand<Grid>(OnClearPassword);
            ManageSignaturesCommand = new DelegateCommand<object>(OnManageSignatures);
        }

        protected override void Load()
        {
            Users = new ObservableCollection<User>(Repository.GetAll());
        }

        protected override void InitialiseRepositories()
        {
            Repository = ObjectFactory.GetInstance<IRepository<User>>();
        }

        public override void Save()
        {
            Repository.Save();
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (string.Equals(propertyName, "NewUserName") || string.Equals(propertyName, "NewUserPassword"))
                RefreshCommands();
        }
        
        private void OnAdd(PasswordBox passwordBox)
        {
            bool hasEnteredDetails = !string.IsNullOrEmpty(NewUserName) && !string.IsNullOrEmpty(passwordBox.Password);
            bool exists = UserManagement.UserExists(Repository, NewUserName);

            if (!hasEnteredDetails || exists || Repository.FirstOrDefault(t => string.Equals(t.Username, NewUserName)) != null)
            {
                MessageBoxHelper.ShowError(Resources.TXT_ERR_ENTER_UNIQUE_USERNAME);
                return;
            }

            UserManagement.AddUser(Repository, NewUserName, passwordBox.Password);
            Repository.Save();
            Users = new ObservableCollection<User>(Repository.GetAll());
            OnClear(passwordBox);
        }

        private void OnClear(PasswordBox passwordBox)
        {
            if (passwordBox == null)
                return;

            NewUserName = passwordBox.Password = string.Empty;
        }

        private void OnChangePassword(Grid grid)
        {
            if (grid == null)
                return;

            IDictionary<string, string> passwords = GetPasswords(grid);
            string old = passwords["Old"];
            string newPassword = passwords["New"];

            bool changed = UserManagement.ChangePassword(Repository, old, newPassword);
            if (changed)
                MessageBoxHelper.ShowMessage(Resources.TXT_PASSWORD_HAS_BEEN_CHANGED);
            else
                MessageBoxHelper.ShowError(Resources.ERR_COULD_NOT_CHANGE_PASSWORD);

            ClearPasswords(grid);
        }
        
        private void OnResetPassword(Grid grid)
        {
            string currentUser = UserManagement.LoggedInUserName;
            if (currentUser != "superuser")
            {
                MessageBoxHelper.ShowMessage(Resources.ERR_PASSWORD_HAS_TO_BE_RESET_BY_SUPERUSER);
                return;
            }
            if (currentUser == SelectedUser.Username)
            {
                MessageBoxHelper.ShowMessage(Resources.ERR_DO_NOT_RESET_OWN_PASSWORD);
                return;
            }
            if (grid == null)
                return;

            string old = SelectedUser.Password;
            const string newPassword = "password";

            bool changed = UserManagement.ResetPassword(Repository, old, newPassword);
            if (changed)
                MessageBoxHelper.ShowMessage(Resources.TXT_PASSWORD_HAS_BEEN_RESET);
            else
                MessageBoxHelper.ShowError(Resources.ERR_COULD_NOT_CHANGE_PASSWORD);

            ClearPasswords(grid);
        }

        private void OnClearPassword(Grid grid)
        {
            if (grid == null)
                return;

            ClearPasswords(grid);
        }

        private void OnManageSignatures(object obj)
        {
            var window = new SignatureCaptureWindow();
            window.ShowDialog();
        }
        
        private void RefreshCommands()
        {
            AddCommand.RaiseCanExecuteChanged();
            ClearCommand.RaiseCanExecuteChanged();
        }

        private void ClearPasswords(DependencyObject root)
        {
            if (root == null)
                return;

            IEnumerable<PasswordBox> children = root.FindVisualChildren<PasswordBox>();
            foreach (PasswordBox child in children)
            {
                child.Password = string.Empty;
            }
        }

        private IDictionary<string, string> GetPasswords(DependencyObject root)
        {
            if (root == null)
                return null;

            var result = new Dictionary<string, string>();

            IEnumerable<PasswordBox> children = root.FindVisualChildren<PasswordBox>();
            foreach (PasswordBox child in children)
            {
                if (string.Equals(child.Name, "OldPasswordBox"))
                    result.Add("Old", child.Password);
                else if (string.Equals(child.Name, "NewPasswordBox"))
                    result.Add("New", child.Password);
            }

            return result;
        }
    }
}