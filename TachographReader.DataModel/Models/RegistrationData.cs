namespace TachographReader.DataModel
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Globalization;
    using Connect.Shared;
    using Connect.Shared.Models;
    using Shared.Connect;
    using Shared.Helpers;

    [Table("RegistrationData")]
    public class RegistrationData : BaseModel
    {
        [NotMapped]
        public Action Updated { get; set; }

        public string LicenseKey { get; set; }

        public bool HasValidLicense
        {
            get { return !string.IsNullOrEmpty(LicenseKey) && ExpirationDate.HasValue && ExpirationDate.Value >= DateTime.Now; }
        }

        public bool HasUnexpiredLicense
        {
            get { return string.IsNullOrEmpty(LicenseKey) || (ExpirationDate.GetValueOrDefault() >= DateTime.Now); }
        }

        public DateTime? ExpirationDate
        {
            get { return LicenseManager.GetExpirationDate(LicenseKey); }
        }

        public string CompanyName { get; set; }
        public string SealNumber { get; set; }
        public string DepotName { get; set; }

        public string WebcalConnectKey
        {
            get { return $"{CompanyName}-{LicenseManager.GetMachineKey()}-{LicenseKey}-{DepotName}"; }
        }

        public bool IsConnectEnabled { get; set; }

        [NotMapped]
        public IConnectKeys ConnectKeys
        {
            get
            {
                var licenseKey = 0;

                if (ExpirationDate != null)
                {
                    int.TryParse(ExpirationDate.GetValueOrDefault().Ticks.ToString(CultureInfo.InvariantCulture).TrimEnd('0'), out licenseKey);
                }

                return new ConnectKeys(ConnectUrlHelper.ServiceUrl, licenseKey, CompanyName, LicenseManager.GetMachineKey(), DepotName);
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