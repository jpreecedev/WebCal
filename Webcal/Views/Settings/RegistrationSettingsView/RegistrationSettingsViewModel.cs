namespace Webcal.Views.Settings
{
    using System;
    using System.Linq;
    using System.Windows.Controls;
    using Controls;
    using Core;
    using DataModel;
    using Properties;
    using Shared;
    using StructureMap;

    public class RegistrationSettingsViewModel : BaseSettingsViewModel
    {
        private string _serial;
        
        public RegistrationSettingsViewModel()
        {
            TextField = new InputTextField {Label = Resources.TXT_LICENSE_KEY};

            var binding = new InputBinding("Serial") {Source = this};
            TextField.SetBinding(TextBox.TextProperty, binding);
        }
        
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
    }
}