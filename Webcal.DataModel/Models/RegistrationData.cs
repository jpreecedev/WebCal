namespace Webcal.DataModel
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Shared;
    using Shared.Core;
    using Shared.Helpers;

    [Table("RegistrationData")]
    public class RegistrationData : BaseModel
    {
        public string LicenseKey { get; set; }

        public DateTime? ExpirationDate
        {
            get { return LicenseManager.GetExpirationDate(LicenseKey); }
        }

        public string CompanyName { get; set; }
        public string SealNumber { get; set; }
        public string WebcalConnectKey { get; set; }
    }
}