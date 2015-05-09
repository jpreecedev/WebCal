namespace TachographReader.Shared
{
    using System;
    using System.Collections.Generic;
    using global::Connect.Shared.Models;

    public class Report : BaseNotification
    {
        private readonly string _calibrationsDueText;
        private readonly string _recentCalibrationsText;

        public Report(string recentCalibrationsText, string calibrationsDueText)
        {
            _recentCalibrationsText = recentCalibrationsText;
            _calibrationsDueText = calibrationsDueText;
        }

        public string ReportType { get; set; }
        public string DocumentType { get; set; }
        public IList<string> Technicians { get; set; }
        public IList<string> VehicleManufacturers { get; set; }
        public string RegistrationNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == "ReportType")
            {
                if (ReportType == _recentCalibrationsText)
                {
                    FromDate = DateTime.Now.AddMonths(-2);
                    ToDate = DateTime.Now;
                }
                else if (ReportType == _calibrationsDueText)
                {
                    FromDate = DateTime.Now;
                    ToDate = DateTime.Now.AddMonths(2);
                }
            }
        }
    }
}