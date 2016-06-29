namespace TachographReader.Core
{
    using System.Windows;
    using DataModel.Library;
    using Library;

    public class BaseSettingsViewModel : BaseViewModel
    {
        public BaseSettingsViewModel()
        {
            SettingsRepoHack = SettingsRepoHack.GetInstance();
        }

        public SettingsRepoHack SettingsRepoHack { get; set; }

        public Visibility LoggedInAsSuperUser
        {
            get { return UserManagement.LoggedInAsSuperUser ? Visibility.Visible : Visibility.Collapsed; }
        }

        public virtual void Save()
        {
        }
    }
}