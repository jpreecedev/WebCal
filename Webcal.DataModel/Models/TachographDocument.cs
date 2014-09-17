namespace Webcal.DataModel
{
    using System;
    using Library;
    using Properties;
    using Shared;

    public class TachographDocument : Document
    {
        [Macro(Placeholder = "{VIN}", Meaning = "Vehicle Identification Number")]
        public string VIN { get; set; }

        [Macro(Placeholder = "{VehMake}", Meaning = "Vehicle Make")]
        public string VehicleMake { get; set; }

        [Macro(Placeholder = "{VehModel}", Meaning = "Vehicle Model")]
        public string VehicleModel { get; set; }

        [Macro(Placeholder = "{TyreSize}", Meaning = "Tyre Size")]
        public string TyreSize { get; set; }

        [Macro(Placeholder = "{VehType}", Meaning = "Vehicle Type")]
        public string VehicleType { get; set; }

        [Macro(Placeholder = "{WFac}", Meaning = "W Factor")]
        public string WFactor { get; set; }

        [Macro(Placeholder = "{KFac}", Meaning = "K Factor")]
        public string KFactor { get; set; }

        [Macro(Placeholder = "{LFac}", Meaning = "L Factor")]
        public string LFactor { get; set; }

        [Macro(Placeholder = "{Odo}", Meaning = "Odometer Reading")]
        public string OdometerReading { get; set; }

        public bool Tampered { get; set; }

        [Macro(Placeholder = "{InvNo}", Meaning = "Invoice Number")]
        public string InvoiceNumber { get; set; }

        [Macro(Placeholder = "{InspInfo}", Meaning = "Inspection Info")]
        public string InspectionInfo { get; set; }

        public bool TachographHasAdapter { get; set; }

        public string TachographAdapterSerialNumber { get; set; }

        [Macro(Placeholder = "{AdaptLoc}", Meaning = "Adapter Location")]
        public string TachographAdapterLocation { get; set; }

        public string TachographCableColor { get; set; }

        [Macro(Placeholder = "{MinWork}", Meaning = "Minor Work Details")]
        public string MinorWorkDetails { get; set; }

        [Macro(Placeholder = "{TachoType}", Meaning = "Tachograph Type")]
        public string TachographType { get; set; }

        [Macro(Placeholder = "{CardSerial}", Meaning = "Card Serial Number")]
        public string CardSerialNumber { get; set; }

        [Macro(Placeholder = "{CalTime}", Meaning = "Calibration Date/Time")]
        public DateTime? CalibrationTime { get; set; }

        public bool IsDigital { get; set; }

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

        public void Convert(CalibrationRecord calibrationRecord)
        {
            if (calibrationRecord == null)
                return;

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