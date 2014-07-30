using System;
using System.Linq;
using System.Windows.Controls;
using StructureMap;
using Webcal.Controls;
using Webcal.Core;
using Webcal.DataModel;
using Webcal.Shared;
using Webcal.Properties;

namespace Webcal.Views.Settings
{
    public class RegistrationSettingsViewModel : BaseSettingsViewModel
    {
        private string _serial;

        #region Constructor

        public RegistrationSettingsViewModel()
        {
            TextField = new InputTextField {Label = Resources.TXT_LICENSE_KEY};

            InputBinding binding = new InputBinding("Serial") {Source = this};
            TextField.SetBinding(TextBox.TextProperty, binding);
        }

        #endregion

        #region Public Properties

        public IRepository<RegistrationData> Repository { get; set; }

        public RegistrationData Settings
        {
            get
            {
                if (Repository == null)
                    return null;

                return Repository.GetAll().First();
            }
        }

        public InputTextField TextField { get; set; }

        public DateTime ExpirationDateTime { get; set; }

        public string Serial
        {
            get { return _serial; }
            set
            {
                _serial = value;
                SerialChanged();
            }
        }

        #endregion

        #region Overrides

        protected override void InitialiseRepositories()
        {
            Repository = ObjectFactory.GetInstance<IRepository<RegistrationData>>();
        }

        protected override void Load()
        {
            if (!string.IsNullOrEmpty(Settings.LicenseKey))
                Serial = Settings.LicenseKey;
        }

        public override void Save()
        {
            Repository.Save();
        }

        #endregion

        #region Private Methods

        private void SerialChanged()
        {
            DateTime expirationDate;
            if (!LicenseManager.IsValid(Serial, out expirationDate))
            {
                ExpirationDateTime = default(DateTime);
                TextField.Valid = false;
                TextField.IsHighlighted = false;
                return;
            }

            if (LicenseManager.HasExpired(expirationDate))
            {
                ExpirationDateTime = default(DateTime);
                TextField.Valid = false;
                return;
            }

            ExpirationDateTime = expirationDate;
            TextField.Valid = true;
            TextField.IsHighlighted = true;
            Settings.LicenseKey = Serial;
        }

        #endregion
    }
}