namespace TachographReader.Views.Settings
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using Connect.Shared;
    using Core;
    using Properties;

    public class WorkshopSettingsViewModel : BaseSettingsViewModel
    {
        public WorkshopSettingsViewModel()
        {
            BrowseCommand = new DelegateCommand<object>(OnBrowse);
        }
        
        public DelegateCommand<object> BrowseCommand { get; set; }

        public WorkshopSettings Settings
        {
            get { return SettingsRepoHack.Settings; }
        }

        public override void Save()
        {
            SettingsRepoHack.SaveSettings();
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