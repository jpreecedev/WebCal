namespace TachographReader.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;
    using Core;
    using DataModel;
    using Library.PDF;
    using Properties;
    using Shared;

    public class GenerateReportViewModel : BaseNavigationViewModel
    {
        public IRepository<Technician> TechniciansRepository { get; set; }
        public IRepository<VehicleMake> VehicleMakesRepository { get; set; }

        public DelegateCommand<Grid> GenerateReportCommand { get; set; }

        public ICollection<string> ReportTypes { get; set; }
        public ICollection<string> DocumentTypes { get; set; }
        public ICollection<string> Technicians { get; set; }
        public ICollection<string> VehicleManufacturers { get; set; }

        public Report Report { get; set; }

        protected override void InitialiseCommands()
        {
            GenerateReportCommand = new DelegateCommand<Grid>(OnGenerateReport);
        }

        protected override void InitialiseRepositories()
        {
            TechniciansRepository = GetInstance<IRepository<Technician>>();
            VehicleMakesRepository = GetInstance<IRepository<VehicleMake>>();
        }

        protected override void Load()
        {
            ReportTypes = new List<string>
            {
                Resources.TXT_RECENT_CALIBRATIONS, 
                Resources.TXT_CALIBRATIONS_DUE
            };

            DocumentTypes = new List<string>
            {
                Resources.TXT_ANY, 
                Resources.TXT_DIGITAL_TACHOGRAPH, 
                Resources.TXT_ANALOGUE_TACHOGRAPH,
                Resources.TXT_UNDOWNLOADABILITY, 
                Resources.TXT_LETTER_FOR_DECOMMISSIONING
            };

            Technicians = TechniciansRepository.GetAll().Where(c => c != null).OrderBy(c => c.Name).Select(c => c.Name).ToList();
            VehicleManufacturers = VehicleMakesRepository.GetAll().Where(c => c != null).OrderBy(c => c.Name).Select(c => c.Name).ToList();

            Report = new Report(Resources.TXT_RECENT_CALIBRATIONS, Resources.TXT_CALIBRATIONS_DUE)
            {
                ReportType = ReportTypes.First(),
                DocumentType = DocumentTypes.First(),
                Technicians = new List<string>(),
                VehicleManufacturers = new List<string>(),
                FromDate = DateTime.Now.AddMonths(-2),
                ToDate = DateTime.Now
            };
        }

        private void OnGenerateReport(Grid root)
        {
            if (!IsValid(root))
            {
                ShowError(Resources.EXC_MISSING_FIELDS);
                return;
            }

            PDFDocumentResult result = Report.ToPDF();
            if (result.Success)
            {
                if (AskQuestion(Resources.TXT_DO_YOU_WANT_TO_PRINT))
                {
                    result.Print();
                }
            }
        }
    }
}