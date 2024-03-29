﻿namespace TachographReader.Views.Settings
{
    using System;
    using System.Linq;
    using Connect.Shared;
    using Controls;
    using Core;
    using DataModel;
    using DataModel.Core;
    using Library;
    using Properties;
    using Shared;
    using Shared.Connect;
    using Shared.Helpers;

    public class RegistrationSettingsViewModel : BaseSettingsViewModel
    {
        private string _serial;
        private RegistrationData _registrationData;

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

                return _registrationData ?? (_registrationData = Repository.GetAll().First());
            }
        }

        public InputTextField LicenseKeyField { get; set; }
        public InputTextField WebcalConnectField { get; set; }

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

        public string WebcalConnectKey
        {
            get { return Settings.WebcalConnectKey; }
        }

        protected override void InitialiseRepositories()
        {
            Repository = GetInstance<IRepository<RegistrationData>>();
        }

        protected override void InitialiseCommands()
        {
            ConnectCommand = new DelegateCommand<object>(OnConnect);
        }

        protected override void Load()
        {
            LicenseKeyField = InputTextField.CreateInputTextField<string, InputBinding>(Resources.TXT_LICENSE_KEY, () => Serial, this);
            WebcalConnectField = InputTextField.CreateInputTextField<string, OneWayInputBinding>(Resources.TXT_WEBCAL_CONNECT_KEY, () => WebcalConnectKey, this, ConnectCommand, Resources.TXT_VERIFY, true);

            if (!string.IsNullOrEmpty(Settings.LicenseKey))
            {
                Serial = Settings.LicenseKey;
            }

            Settings.Updated = () => OnPropertyChanged("WebcalConnectKey");
        }

        public override void Save()
        {
            ConnectHelper.ConnectKeysChanged();
            Repository.AddOrUpdate(Settings);
        }

        private void SerialChanged()
        {
            DateTime expirationDate;
            if (!LicenseManager.IsValid(Serial, out expirationDate) || LicenseManager.HasExpired(expirationDate))
            {
                ExpirationDateTime = default(DateTime);
                LicenseKeyField.Valid = false;
                LicenseKeyField.IsHighlighted = false;
            }
            else
            {
                ExpirationDateTime = expirationDate;
                LicenseKeyField.Valid = true;
                LicenseKeyField.IsHighlighted = true;
            }

            Settings.LicenseKey = Serial;
            Repository.AddOrUpdate(Settings);
        }

        private void OnConnect(object obj)
        {
            var webcalConnectKey = WebcalConnectKey.Split('-');
            var companyName = webcalConnectKey[0];
            var machineKey = webcalConnectKey[1];
            var depotKey = webcalConnectKey[3];

            int expiration;
            int.TryParse(webcalConnectKey[2], out expiration);

            var connectKeys = new ConnectKeys(ConnectUrlHelper.ServiceUrl, expiration, companyName, machineKey, depotKey);
            WebcalConnectField.IsLoading = true;

            GetInstance<IConnectClient>().CallAsync(connectKeys, client =>
            {
                client.Service.Echo();
            },
            result =>
            {
                WebcalConnectField.Valid = WebcalConnectField.IsHighlighted = result.IsSuccess;
            },
            exception =>
            {
                WebcalConnectField.Valid = WebcalConnectField.IsHighlighted = false;
                ShowError(ExceptionPolicy.HandleException(ContainerBootstrapper.Container, exception));
            },
            () =>
            {
                WebcalConnectField.IsLoading = false;
            });
        }
    }
}