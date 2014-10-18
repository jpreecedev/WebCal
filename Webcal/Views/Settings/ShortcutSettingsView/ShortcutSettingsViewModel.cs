namespace Webcal.Views.Settings
{
    using Controls;
    using Core;

    public class ShortcutSettingsViewModel : BaseViewModel
    {
        public DelegateCommand<Shortcut> ShortcutCommand { get; set; }

        protected override void InitialiseCommands()
        {
            ShortcutCommand = new DelegateCommand<Shortcut>(OnShortcut);
        }

        private static void OnShortcut(Shortcut shortcut)
        {
            if (shortcut == null || shortcut.SettingsViewModel == null)
            {
                return;
            }

            BaseNavigationViewModel navigationVM = shortcut.SettingsViewModel;
            if (navigationVM != null)
            {
                navigationVM.ShowSettingsView(shortcut.View);
                shortcut.SettingsViewModel.SetSelectedTreeViewItem(shortcut.View);
            }
        }
    }
}