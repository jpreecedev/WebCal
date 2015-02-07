namespace Webcal.DataModel
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Globalization;
    using Connect.Shared;
    using Connect.Shared.Models;
    using Shared;
    using Shared.Helpers;

    [Table("RegistrationData")]
    public class RegistrationData : BaseModel
    {
        [NotMapped]
        public Action Updated { get; set; }

        public string LicenseKey { get; set; }

        public DateTime? ExpirationDate
        {
            get { return LicenseManager.GetExpirationDate(LicenseKey); }
        }

        public string CompanyName { get; set; }
        public string SealNumber { get; set; }

        public string WebcalConnectKey
        {
            get { return string.Format("{0}-{1}-{2}", CompanyName, LicenseManager.GetMachineKey(), ExpirationDate.GetValueOrDefault().Ticks.ToString(CultureInfo.InvariantCulture).TrimEnd('0')); }
        }

        public bool IsConnectEnabled { get; set; }

        [NotMapped]
        public IConnectKeys ConnectKeys
        {
            get
            {
                string url = WebcalConfigurationSection.Instance.GetConnectUrl();
                var licenseKey = int.Parse(ExpirationDate.GetValueOrDefault().Ticks.ToString(CultureInfo.InvariantCulture).TrimEnd('0'));

                return new ConnectKeys(url, licenseKey, CompanyName, LicenseManager.GetMachineKey());
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (Updated != null)
            {
                Updated();
            }
        }
    }
}