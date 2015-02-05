namespace Webcal.DataModel
{
    using System;
    using System.Xml.Serialization;
    using Library;
    using Properties;

    [Serializable]
    public class TachographDocument : Document
    {
        [XmlIgnore]
        public override bool IsNew
        {
            get
            {
                return string.IsNullOrEmpty(RegistrationNumber) &&
                       string.IsNullOrEmpty(VIN) &&
                       string.IsNullOrEmpty(VehicleMake) &&
                       string.IsNullOrEmpty(VehicleModel) &&
                       string.IsNullOrEmpty(TyreSize) &&
                       string.IsNullOrEmpty(WFactor) &&
                       string.IsNullOrEmpty(KFactor) &&
                       string.IsNullOrEmpty(LFactor);
            }
        }

        public string VIN { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public string TyreSize { get; set; }
        public string VehicleType { get; set; }
        public string WFactor { get; set; }
        public string KFactor { get; set; }
        public string LFactor { get; set; }
        public string OdometerReading { get; set; }
        public bool Tampered { get; set; }
        public string InvoiceNumber { get; set; }
        public string InspectionInfo { get; set; }
        public bool TachographHasAdapter { get; set; }
        public string TachographAdapterSerialNumber { get; set; }
        public string TachographAdapterLocation { get; set; }
        public string TachographCableColor { get; set; }
        public string MinorWorkDetails { get; set; }
        public string TachographType { get; set; }
        public string CardSerialNumber { get; set; }
        public DateTime? CalibrationTime { get; set; }
        public bool IsDigital { get; set; }

        public void Convert(CalibrationRecord calibrationRecord)
        {
            if (calibrationRecord == null)
            {
                return;
            }

            VIN = calibrationRecord.VehicleIdentificationNumber;
            KFactor = calibrationRecord.KFactor;
            WFactor = calibrationRecord.WFactor;
            RegistrationNumber = calibrationRecord.VehicleRegistrationNumber;
            OdometerReading = calibrationRecord.OdometerValue;
            TyreSize = calibrationRecord.TyreSize;
            LFactor = calibrationRecord.TyreCircumference;
            TachographMake = calibrationRecord.TachographManufacturer;
            SerialNumber = calibrationRecord.VuSerialNumber;
            CardSerialNumber = calibrationRecord.CardSerialNumber;
            CalibrationTime = calibrationRecord.CalibrationTime;
        }

        protected override void OnDocumentTypeChanged(string newValue)
        {
            if (IsNew && string.Equals(DocumentType, Resources.TXT_MINOR_WORK_DETAILS) && string.IsNullOrEmpty(MinorWorkDetails))
            {
                MinorWorkDetails = Resources.TXT_ACTIVITY_MODE_CHANGE;
            }
        }
    }
}