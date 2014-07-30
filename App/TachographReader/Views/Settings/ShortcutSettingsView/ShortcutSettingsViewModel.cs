using Webcal.Controls;
using Webcal.Core;

namespace Webcal.Views.Settings
{
    public class ShortcutSettingsViewModel : BaseViewModel
    {
        #region Overrides

        protected override void InitialiseCommands()
        {
            ShortcutCommand = new DelegateCommand<Shortcut>(OnShortcut);
        }

        #endregion

        #region Commands

        #region Command : Shortcut

        public DelegateCommand<Shortcut> ShortcutCommand { get; set; }

        private static void OnShortcut(Shortcut shortcut)
        {
            if (shortcut == null || shortcut.SettingsViewModel == null)
                return;

            BaseNavigationViewModel navigationVM = shortcut.SettingsViewModel;
            if (navigationVM != null)
            {
                navigationVM.ShowSettingsView(shortcut.View);
                shortcut.SettingsViewModel.SetSelectedTreeViewItem(shortcut.View);
            }
        }

        #endregion

        #endregion
    }
}