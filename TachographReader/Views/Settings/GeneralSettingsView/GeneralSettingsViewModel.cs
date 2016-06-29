namespace TachographReader.Views.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Forms;
    using Connect.Shared;
    using Core;
    using Properties;

    public class GeneralSettingsViewModel : BaseSettingsViewModel
    {
        public GeneralSettingsViewModel()
        {
            DaysOfWeek = new ObservableCollection<CustomDayOfWeek>
            {
                new CustomDayOfWeek {DayOfWeek = Resources.TXT_MONDAY},
                new CustomDayOfWeek {DayOfWeek = Resources.TXT_TUESDAY},
                new CustomDayOfWeek {DayOfWeek = Resources.TXT_WEDNESDAY},
                new CustomDayOfWeek {DayOfWeek = Resources.TXT_THURSDAY},
                new CustomDayOfWeek {DayOfWeek = Resources.TXT_FRIDAY},
                new CustomDayOfWeek {DayOfWeek = Resources.TXT_SATURDAY},
                new CustomDayOfWeek {DayOfWeek = Resources.TXT_SUNDAY}
            };
        }

        public WorkshopSettings Settings
        {
            get { return SettingsRepoHack.Settings; }    
        }

        public ICollection<CustomDayOfWeek> DaysOfWeek { get; set; }
        public DelegateCommand<object> BrowseCommand { get; set; }

        protected override void Load()
        {
            if (Settings.CustomDayOfWeeks == null)
            {
                return;
            }

            foreach (CustomDayOfWeek dayOfWeek in Settings.CustomDayOfWeeks)
            {
                foreach (CustomDayOfWeek customDay in DaysOfWeek)
                {
                    if (dayOfWeek.DayOfWeek == customDay.DayOfWeek)
                    {
                        customDay.IsChecked = true;
                    }
                }
            }
        }

        protected override void InitialiseCommands()
        {
            BrowseCommand = new DelegateCommand<object>(OnBrowse);
        }
        
        public override void Save()
        {
            Settings.CustomDayOfWeeks = DaysOfWeek.Where(d => d.IsChecked).ToList();
            SettingsRepoHack.SaveSettings();
        }

        private void OnBrowse(object obj)
        {
            var folderBrowserDialog = new FolderBrowserDialog { SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) };
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                Settings.BackupFilePath = folderBrowserDialog.SelectedPath;
            }
        }
    }
}