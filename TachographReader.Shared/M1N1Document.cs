namespace TachographReader.Shared
{
    using System;
    using global::Connect.Shared.Models;

    public class M1N1Document : BaseNotification
    {
        public M1N1Document()
        {
            DateOfCalibration = DateTime.Now;
        }

        public string CompanyName { get; set; }

        public string PhoneNumber { get; set; }

        public string DocumentType { get; set; }

        public DateTime DateOfCalibration { get; set; }

        public string KFactor { get; set; }

        public string WFactor { get; set; }

        public string LFactor { get; set; }

        public string VIN { get; set; }

        public string SerialNumber { get; set; }

        public string SealNumber { get; set; }

        public string TyreSize { get; set; }

        public string SecondCompanyName { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }
    }
}