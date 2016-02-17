namespace TachographReader.Library.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using Connect.Shared.Models;
    using Properties;

    public class QCReportViewModel : QCReport
    {
        public static string InitialInstallationAnalogue = Resources.TXT_NEW_QC_CHECK_INITIAL_INSTALLATION_ANALOGUE;
        public static string SixYearCalibrationAnalogue = Resources.TXT_NEW_QC_SIX_YEAR_ANALOGUE;
        public static string TwoYearInspectionAnalogue = Resources.TXT_NEW_QC_2_YEAR_ANALOGUE;
        public static string InitialInstallationDigital = Resources.TXT_NEW_QC_INITIAL_DIGITAL;
        public static string TwoYearCalibrationDigital = Resources.TXT_NEW_QC_2_CALIBRATION_DIGITAL;

        public QCReportViewModel()
        {
            ReportTypes = new ObservableCollection<string>
            {
                InitialInstallationAnalogue,
                SixYearCalibrationAnalogue,
                TwoYearInspectionAnalogue,
                InitialInstallationDigital,
                TwoYearCalibrationDigital
            };

            if (DateOfAudit == default(DateTime))
            {
                DateOfAudit = DateTime.Now;
            }
        }

        public QCReportViewModel(QCReport report) : this()
        {
            if (report == null)
            {
                return;
            }

            TachoCentreLine1 = report.TachoCentreLine1;
            TachoCentreLine2 = report.TachoCentreLine2;
            TachoCentreLine3 = report.TachoCentreLine3;
            TachoCentreCity = report.TachoCentreCity;
            TachoCentrePostCode = report.TachoCentrePostCode;
            TachoManagerName = report.TachoManagerName;
            QCManagerName = report.QCManagerName;
            TechnicianName = report.TechnicianName;
            DateOfAudit = report.DateOfAudit;
            TypeOfTachographCheck = report.TypeOfTachographCheck;
            TachographMake = report.TachographMake;
            TachographModel = report.TachographModel;
            TachographSerialNumber = report.TachographSerialNumber;
            VehicleMake = report.VehicleMake;
            VehicleType = report.VehicleType;
            VehicleRegistrationNumber = report.VehicleRegistrationNumber;
            VehicleIdentificationNumber = report.VehicleIdentificationNumber;
            ThreeBasicChecksCompleted = report.ThreeBasicChecksCompleted;
            WFactor = report.WFactor;
            LFactor = report.LFactor;
            KFactor = report.KFactor;
            FortyKmTest = report.FortyKmTest;
            SixtyKmTest = report.SixtyKmTest;
            ClockTestCompleted = report.ClockTestCompleted;
            BenchTestCarriedOutAnalogue = report.BenchTestCarriedOutAnalogue;
            FunctionalBenchTestDigital = report.FunctionalBenchTestDigital;
            DistanceCheckCarriedOut = report.DistanceCheckCarriedOut;
            TestChartsCompleted = report.TestChartsCompleted;
            SpeedForSpeedCheckCompleted = report.SpeedForSpeedCheckCompleted;
            SystemSealedInAccordance = report.SystemSealedInAccordance;
            CalibrationCertificateCompleted = report.CalibrationCertificateCompleted;
            ReferenceCableCheckCompleted = report.ReferenceCableCheckCompleted;
            TechnicalDataPrintoutsCreated = report.TechnicalDataPrintoutsCreated;
            EventsFaultsReadCleared = report.EventsFaultsReadCleared;
            Comments = report.Comments;
            Passed = report.Passed;
        }

        public ObservableCollection<string> ReportTypes { get; set; }

        public bool IsDocumentType(string documentType)
        {
            return TypeOfTachographCheck == documentType;
        }
    }
}