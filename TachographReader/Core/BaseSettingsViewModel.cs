namespace TachographReader.Core
{
    using System.Windows;
    using DataModel.Library;

    public class BaseSettingsViewModel : BaseViewModel
    {
        public Visibility LoggedInAsSuperUser
        {
            get { return UserManagement.LoggedInAsSuperUser ? Visibility.Visible : Visibility.Collapsed; }
        }

        public virtual void Save()
        {
        }
    }
}