namespace TachographReader.Views.Settings
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using Core;
    using Properties;
    using TachographReader.DataModel.Models;
    using TachographReader.Shared;
    using System.Collections.ObjectModel;
    using DataModel.Library;
    using Library;

    public class AdvertisingSettingsViewModel : BaseSettingsViewModel
    {
        public AdvertisingSettings Settings { get; set; }
        public ISettingsRepository<AdvertisingSettings> SettingsRepository { get; set; }
        public ObservableCollection<string> Fonts { get; set; }

        public DelegateCommand<object> BrowseCommand { get; set; }
        public DelegateCommand<object> ClearCommand { get; set; }

        protected override void InitialiseCommands()
        {
            BrowseCommand = new DelegateCommand<object>(OnBrowse);
            ClearCommand = new DelegateCommand<object>(OnClear);
        }

        protected override void InitialiseRepositories()
        {
            SettingsRepository = GetInstance<ISettingsRepository<AdvertisingSettings>>();
        }

        protected override void Load()
        {
            Settings = SettingsRepository.GetAdvertisingSettings();
            Fonts = new ObservableCollection<string>(FontHelper.GetInstalledFonts());
        }

        public override void Save()
        {
            SettingsRepository.Save(Settings);
        }

        private void OnBrowse(object obj)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = Resources.SELECT_BITMAP_IMAGE,
                CheckPathExists = true,
                CheckFileExists = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Settings.Image = Image.FromFile(openFileDialog.FileName);
            }
        }

        private void OnClear(object obj)
        {
            Settings.Image = null;
        }
    }
}