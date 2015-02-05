namespace Webcal.DataModel
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Globalization;
    using Connect.Shared.Models;
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