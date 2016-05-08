namespace TachographReader.Views.Settings
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using Connect.Shared;
    using Core;
    using DataModel.Library;
    using Properties;
    using Shared;

    public class WorkshopSettingsViewModel : BaseSettingsViewModel
    {
        public WorkshopSettingsViewModel()
        {
            GeneralSettingsRepository = GetInstance<ISettingsRepository<WorkshopSettings>>();
            BrowseCommand = new DelegateCommand<object>(OnBrowse);
        }

        public ISettingsRepository<WorkshopSettings> GeneralSettingsRepository { get; set; }
        public DelegateCommand<object> BrowseCommand { get; set; }
        public WorkshopSettings Settings { get; set; }

        protected override void Load()
        {
            Settings = GeneralSettingsRepository.GetWorkshopSettings();
        }

        public override void Save()
        {
            GeneralSettingsRepository.Save(Settings);
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
    }
}