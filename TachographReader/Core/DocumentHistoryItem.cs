namespace TachographReader.Core
{
    using System;
    using Connect.Shared.Models;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using Library;
    using Library.PDF;
    using Shared;

    public class DocumentHistoryItem : IDocumentHistoryItem
    {
        public DocumentHistoryItem(Document document)
        {
            Type = document.GetType().Name.SplitByCapitals();
            Created = document.Created;
            DocumentType = document.DocumentType;
            RegistrationNumber = document.RegistrationNumber;
            TechnicianName = document.Technician;
            Office = document.Office;
            CustomerContact = document.CustomerContact;
            Document = document;

            CanReprintLabel = Document is TachographDocument;
            CanPrintGV212Document = Document is TachographDocument;
        }

        public DocumentHistoryItem(BaseReport report)
        {
            var qcReport = report as QCReport;
            if (qcReport != null)
            {
                FromQCReport(qcReport);
            }

            var month = report as QCReport6Month;
            if (month != null)
            {
                FromQCReport6Month(month);
            }

            Report = report;
        }

        public string Type { get; set; }
        public DateTime Created { get; set; }
        public string DocumentType { get; set; }
        public string RegistrationNumber { get; set; }
        public string TechnicianName { get; set; }
        public string Office { get; set; }
        public string CustomerContact { get; set; }
        public bool CanReprintLabel { get; set; }
        public Document Document { get; set; }
        public BaseReport Report { get; set; }
        public bool CanPrintGV212Document { get; set; }

        public void Print()
        {
            var miscellaneousSettings = ContainerBootstrapper.Resolve<ISettingsRepository<MiscellaneousSettings>>().GetMiscellaneousSettings();

            if (Document != null)
            {
                Document.ToPDF(excludeLogos: miscellaneousSettings.ExcludeLogosWhenPrinting).Print();
            }
            if (Report != null)
            {
                Report.ToReportPDF(excludeLogos: miscellaneousSettings.ExcludeLogosWhenPrinting).Print();
            }
        }

        public void PrintLabel()
        {
            var document = Document as TachographDocument;
            if (Document != null && document != null)
            {
                LabelHelper.Print(document);
            }
        }

        public void Email()
        {
            var workshopSettings = ContainerBootstrapper.Resolve<ISettingsRepository<WorkshopSettings>>().GetWorkshopSettings();
            var mailSettings = ContainerBootstrapper.Resolve<ISettingsRepository<MailSettings>>().Get();

            if (Document != null)
            {
                Document.ToPDF().Email(workshopSettings, mailSettings);
            }
            if (Report != null)
            {
                Report.ToReportPDF().Email(workshopSettings, mailSettings);
            }
        }

        public bool IsReport()
        {
            return Report != null;
        }

        private void FromQCReport(QCReport report)
        {
            Type = report.GetType().Name.SplitByCapitals();
            Created = report.Created;
            RegistrationNumber = report.VehicleRegistrationNumber;
            TechnicianName = report.TechnicianName;
        }

        private void FromQCReport6Month(QCReport6Month report)
        {
            Type = report.GetType().Name.SplitByCapitals();
            Created = report.Created;
            TechnicianName = report.Name;
        }
    }
}