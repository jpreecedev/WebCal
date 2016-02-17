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

        public ObservableCollection<string> ReportTypes { get; set; }

        public bool IsDocumentType(string documentType)
        {
            return TypeOfTachographCheck == documentType;
        }

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
    }
}