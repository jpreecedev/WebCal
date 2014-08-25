namespace Webcal.Views.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Forms;
    using Core;
    using DataModel;
    using DataModel.Core;
    using StructureMap;

    public class GeneralSettingsViewModel : BaseSettingsViewModel
    {
        public GeneralSettingsViewModel()
        {
            DaysOfWeek = new ObservableCollection<CustomDayOfWeek>
            {
                new CustomDayOfWeek {DayOfWeek = "Monday"},
                new CustomDayOfWeek {DayOfWeek = "Tuesday"},
                new CustomDayOfWeek {DayOfWeek = "Wednesday"},
                new CustomDayOfWeek {DayOfWeek = "Thursday"},
                new CustomDayOfWeek {DayOfWeek = "Friday"},
                new CustomDayOfWeek {DayOfWeek = "Saturday"},
                new CustomDayOfWeek {DayOfWeek = "Sunday"}
            };
        }


        public WorkshopSettings Settings { get; set; }

        public IGeneralSettingsRepository SettingsRepository { get; set; }

        public ICollection<CustomDayOfWeek> DaysOfWeek { get; set; }
        public DelegateCommand<object> BrowseCommand { get; set; }
        
        protected override void Load()
        {
            Settings = SettingsRepository.GetSettings();

            if (Settings.BackupDaysOfWeek == null) return;

            foreach (CustomDayOfWeek dayOfWeek in Settings.BackupDaysOfWeek)
            {
                foreach (CustomDayOfWeek customDay in DaysOfWeek)
                {
                    if (dayOfWeek.DayOfWeek == customDay.DayOfWeek)
                        customDay.IsChecked = true;
                }
            }
        }

        protected override void InitialiseCommands()
        {
            BrowseCommand = new DelegateCommand<object>(OnBrowse);
        }

        protected override void InitialiseRepositories()
        {
            SettingsRepository = ContainerBootstrapper.Container.GetInstance<IGeneralSettingsRepository>();
        }

        public override void Save()
        {
            Settings.BackupDaysOfWeek = DaysOfWeek.Where(d => d.IsChecked).ToList();
            SettingsRepository.Save(Settings);
        }
        
        private void OnBrowse(object obj)
        {
            var folderBrowserDialog = new FolderBrowserDialog {SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)};
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                Settings.BackupFilePath = folderBrowserDialog.SelectedPath;
        }
    }
}