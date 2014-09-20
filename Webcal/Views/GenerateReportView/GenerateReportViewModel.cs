namespace Webcal.Views
{
    using System;
    using System.Windows.Controls;
    using Core;
    using Library;
    using Properties;

    public class GenerateReportViewModel : BaseNavigationViewModel
    {
        private bool _isDocumentExpireNextMonthChecked;
        private bool _isDocumentExpireThisMonthChecked;

        public GenerateReportViewModel()
        {
            IsExpiringDocumentsChecked = true;
            IsExcelFormatChecked = true;
            IsQuickSelectionChecked = true;
            IsDocumentExpireThisMonthChecked = true;
        }

        public bool IsExpiringDocumentsChecked { get; set; }

        public bool IsDocumentStatisticsChecked { get; set; }

        public bool IsQuickSelectionChecked { get; set; }

        public bool IsSelectTimeFrameChecked { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? EndDateTime { get; set; }

        public bool IsTextFormatChecked { get; set; }

        public bool IsExcelFormatChecked { get; set; }

        public string Office { get; set; }

        public bool IsDocumentExpireThisMonthChecked
        {
            get { return _isDocumentExpireThisMonthChecked; }
            set
            {
                _isDocumentExpireThisMonthChecked = value;

                if (value)
                {
                    SetStartEndDates(true);
                }
            }
        }

        public bool IsDocumentExpireNextMonthChecked
        {
            get { return _isDocumentExpireNextMonthChecked; }
            set
            {
                _isDocumentExpireNextMonthChecked = value;

                if (value)
                {
                    SetStartEndDates(false);
                }
            }
        }


        public DelegateCommand<Grid> GenerateReportCommand { get; set; }

        protected override void InitialiseCommands()
        {
            GenerateReportCommand = new DelegateCommand<Grid>(OnGenerateReport);
        }

        private void OnGenerateReport(Grid root)
        {
            if (!IsValid(root))
            {
                ShowError(Resources.EXC_MISSING_FIELDS);
                return;
            }

            ReportFileFormat fileFormat = IsTextFormatChecked ? ReportFileFormat.Text : ReportFileFormat.Excel;

            if (IsDocumentStatisticsChecked)
            {
                ExcelHelper.GenerateDocumentStatistics(fileFormat, Office);
                return;
            }

            if (IsExpiringDocumentsChecked)
            {
                if (StartDateTime == null || EndDateTime == null)
                {
                    return;
                }

                ExcelHelper.GenerateExpiringTachographDocumentsReport(fileFormat, StartDateTime.Value, EndDateTime.Value, Office);
            }
        }

        private void SetStartEndDates(bool thisMonth)
        {
            if (thisMonth)
            {
                StartDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                EndDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
            }
            else
            {
                StartDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1);
                EndDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(2).AddDays(-1);
            }
        }
    }
}