using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StructureMap;
using Webcal.Core;
using Webcal.DataModel.Core;
using Webcal.DataModel;
using Webcal.Properties;

namespace Webcal.Views.Settings
{
    public class WorkshopSettingsViewModel : BaseSettingsViewModel
    {
  
        public WorkshopSettingsViewModel()
        {
            GeneralSettingsRepository = ObjectFactory.GetInstance<IGeneralSettingsRepository>();
            BrowseCommand = new DelegateCommand<object>(OnBrowse);
            BrowseAdvertCommand = new DelegateCommand<object>(OnAdvertBrowse);
            _logoImagePath = ImageFilePath();
            if (File.Exists(_logoImagePath))
            {
                CompanyLogo = BitmapFrame.Create(new Uri(_logoImagePath), BitmapCreateOptions.None,
                    BitmapCacheOption.OnLoad);
            }
            _advertImagePath = AdvertFilePath();
            if (File.Exists(_advertImagePath))
            {
                AdvertisingImage = BitmapFrame.Create(new Uri(_advertImagePath), BitmapCreateOptions.None,
                    BitmapCacheOption.OnLoad);
            }
        }


        public IGeneralSettingsRepository GeneralSettingsRepository { get; set; }

        public DelegateCommand<object> BrowseCommand { get; set; }
        public DelegateCommand<object> BrowseAdvertCommand { get; set; }

        public WorkshopSettings Settings { get; set; }

        private ImageSource _logo;

        private string _logoImagePath;

        public ImageSource CompanyLogo
        {
            get { return _logo; }
            set
            {
                _logo = value;
            }
        }

        private string _advertImagePath;

        private ImageSource _advert;

        public ImageSource AdvertisingImage
        {
            get { return _advert; }
            set
            {
                _advert = value;
            }
        } 

        protected override void Load()
        {
            Settings = GeneralSettingsRepository.GetSettings();
        }

        public override void Save()
        {
            GeneralSettingsRepository.Save(Settings);
        }



        private void OnBrowse(object obj)
        {

            var op = new OpenFileDialog();
            var folderpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WebCal", "ContactImages");

            op.Title = Resources.WorkshopSettingsViewModel_OnBrowse_Select_a_picture;
            op.Filter = Resources.SELECT_BITMAP_IMAGE;

            var myResult = op.ShowDialog() == DialogResult.OK;
            if (myResult)
            {

                if (!Directory.Exists(folderpath)) Directory.CreateDirectory(folderpath);
                foreach (string file in Directory.GetFiles(folderpath))
                {
                    File.Delete(file);
                }


                File.Copy(op.FileName, folderpath + "\\" + Path.GetFileName(op.FileName));
                _logoImagePath = ImageFilePath();
                CompanyLogo = BitmapFrame.Create(new Uri(op.FileName), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }

        }
        
        private static string ImageFilePath()
        {
            var folderpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WebCal", "ContactImages");

            string companyLogo = null;

            if (!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }

            foreach (string file in Directory.GetFiles(folderpath))
            {
                companyLogo = file;
            }
            return companyLogo;
        }






        private void OnAdvertBrowse(object obj)
        {

            var op = new OpenFileDialog();
            var folderpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WebCal", "AdvertImages");

            op.Title = Resources.WorkshopSettingsViewModel_OnBrowse_Select_an_advert;
            op.Filter = Resources.SELECT_BITMAP_IMAGE;

            var myResult = op.ShowDialog() == DialogResult.OK;
            if (myResult)
            {

                if (!Directory.Exists(folderpath)) Directory.CreateDirectory(folderpath);
                foreach (string file in Directory.GetFiles(folderpath))
                {
                    File.Delete(file);
                }


                File.Copy(op.FileName, folderpath + "\\" + Path.GetFileName(op.FileName));
                _advertImagePath = AdvertFilePath();
                AdvertisingImage = BitmapFrame.Create(new Uri(op.FileName), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }

        }



        private static string AdvertFilePath()
        {
            var folderpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WebCal", "AdvertImages");

            string advertLogo = null;

            if (!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }

            foreach (string file in Directory.GetFiles(folderpath))
            {
                advertLogo = file;
            }
            return advertLogo;
        }

    }
}