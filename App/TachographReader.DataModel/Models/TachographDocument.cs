using System;
using Webcal.DataModel.Library;
using Webcal.Shared;

namespace Webcal.DataModel
{
    public class TachographDocument : Document
    {
        #region Public Properties

        [Macro(Placeholder = "{VIN}", Meaning = "Vehicle Identification Number")]
        public string VIN { get; set; }

        [Macro(Placeholder="{VehMake}", Meaning="Vehicle Make")]
        public string VehicleMake { get; set; }

        [Macro(Placeholder="{VehModel}", Meaning="Vehicle Model")]
        public string VehicleModel { get; set; }

        [Macro(Placeholder="{TyreSize}", Meaning="Tyre Size")]
        public string TyreSize { get; set; }

        [Macro(Placeholder="{VehType}", Meaning="Vehicle Type")]
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

        #endregion

        #region Public Methods

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

        #endregion
    }
}
