namespace Webcal.Views.Settings
{
    using System;
    using System.Linq;
    using System.Windows.Controls;
    using Connect;
    using Connect.Shared;
    using Controls;
    using Core;
    using DataModel;
    using DataModel.Core;
    using Properties;
    using Shared;
    using Shared.Connect;
    using Shared.Helpers;

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

        public DelegateCommand<object> ConnectCommand { get; set; }

        public RegistrationData Settings
        {
            get
            {
                if (Repository == null)
                {
                    return null;
                }

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
            Repository = ContainerBootstrapper.Container.GetInstance<IRepository<RegistrationData>>();
        }

        protected override void InitialiseCommands()
        {
            ConnectCommand = new DelegateCommand<object>(OnConnect);
        }

        protected override void Load()
        {
            if (!string.IsNullOrEmpty(Settings.LicenseKey))
            {
                Serial = Settings.LicenseKey;
            }
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

        private void OnConnect(object obj)
        {
            var url = WebcalConfigurationSection.Instance.GetConnectUrl();

            IConnectClient connectClient = new ConnectClient();
            IConnectOperationResult result = connectClient.Open(new ConnectKeys(url, (int) Settings.ExpirationDate.GetValueOrDefault().Ticks, "Skillray", "JP"));
            var a = connectClient.Service.Echo();
            ShowMessage(a, "Webcal Connect");
        }
    }
}