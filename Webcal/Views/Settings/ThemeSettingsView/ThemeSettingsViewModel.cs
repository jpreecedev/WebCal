namespace Webcal.Views.Settings
{
    using System;
    using System.Linq;
    using System.Windows;
    using Core;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using Shared;

    public class ThemeSettingsViewModel : BaseSettingsViewModel
    {
        public ISettingsRepository<ThemeSettings> Repository { get; set; }

        public ThemeSettings ThemeSettings { get; set; }

        public bool SilverTheme { get; set; }

        public bool BlueTheme { get; set; }

        public bool BlackTheme { get; set; }

        protected override void Load()
        {
            SilverTheme = ThemeSettings.SelectedTheme == "Silver";
            BlueTheme = ThemeSettings.SelectedTheme == "Blue";
            BlackTheme = ThemeSettings.SelectedTheme == "Black";
        }

        protected override void InitialiseRepositories()
        {
            Repository = GetInstance<ISettingsRepository<ThemeSettings>>();
            ThemeSettings = Repository.GetThemeSettings();
        }

        public override void Save()
        {
            if (SilverTheme)
            {
                ThemeSettings.SelectedTheme = "Silver";
            }
            else if (BlueTheme)
            {
                ThemeSettings.SelectedTheme = "Blue";
            }
            else if (BlackTheme)
            {
                ThemeSettings.SelectedTheme = "Black";
            }

            var themeDictionary = Application.Current.Resources.MergedDictionaries.First(c => c.Source.ToString().Contains("Office2010"));
            Application.Current.Resources.MergedDictionaries.Remove(themeDictionary);
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = ThemeSettings.Source });

            Repository.Save(ThemeSettings);
        }
    }
}