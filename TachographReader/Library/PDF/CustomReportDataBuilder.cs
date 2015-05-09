namespace TachographReader.Library.PDF
{
    using System.Collections.Generic;
    using System.Linq;
    using Connect.Shared.Models;
    using DataModel.Core;
    using Properties;
    using Shared;

    public static class CustomReportDataBuilder
    {
        public static IEnumerable<Document> GetAllDocuments(Report report)
        {
            if (report == null)
            {
                return null;
            }

            bool recentCalibrations = report.ReportType == Resources.TXT_RECENT_CALIBRATIONS;
            IEnumerable<Document> result = new List<Document>();

            if (report.DocumentType == Resources.TXT_ANY || report.DocumentType == Resources.TXT_DIGITAL_TACHOGRAPH || report.DocumentType == Resources.TXT_ANALOGUE_TACHOGRAPH)
            {
                result = result.Concat(GetDocuments<TachographDocument>(recentCalibrations, report));

                if (report.DocumentType == Resources.TXT_ANALOGUE_TACHOGRAPH)
                {
                    result = result.Where(c => ((TachographDocument)c).IsDigital == false);
                }
            }
            if (report.DocumentType == Resources.TXT_ANY || report.DocumentType == Resources.TXT_UNDOWNLOADABILITY)
            {
                result = result.Concat(GetDocuments<UndownloadabilityDocument>(recentCalibrations, report));
            }
            if (report.DocumentType == Resources.TXT_ANY || report.DocumentType == Resources.TXT_LETTER_FOR_DECOMMISSIONING)
            {
                result = result.Concat(GetDocuments<LetterForDecommissioningDocument>(recentCalibrations, report));
            }

            return result.OrderByDescending(c => c.InspectionDate.GetValueOrDefault());
        }

        private static IEnumerable<T> GetDocuments<T>(bool recentCalibrations, Report report) where T : Document
        {
            var from = report.FromDate;
            var to = report.ToDate;

            var documents = ContainerBootstrapper.Resolve<IRepository<T>>().GetAll().AsEnumerable();
            if (recentCalibrations)
            {
                documents = documents.Where(document => document.Created.Date >= from && document.Created.Date <= to);
            }
            else
            {
                documents = documents.Where(document => document.Created.Date.AddYears(2) >= from && document.Created.Date.AddYears(2) <= to);
            }

            if (!string.IsNullOrEmpty(report.RegistrationNumber))
            {
                documents = documents.Where(document => document.RegistrationNumber == report.RegistrationNumber);
            }
            if (report.Technicians != null && report.Technicians.Count > 0)
            {
                documents = documents.Where(document => report.Technicians.Any(d => d == document.Technician));
            }
            if (report.VehicleManufacturers != null && report.VehicleManufacturers.Count > 0 && typeof(T) == typeof(TachographDocument))
            {
                documents = documents.Where(document => report.VehicleManufacturers.Any(vehicleManufacturer =>
                {
                    var tachographDocument = document as TachographDocument;
                    if (tachographDocument != null)
                    {
                        return tachographDocument.VehicleMake == vehicleManufacturer;
                    }
                    return false;
                }));
            }
            if (!string.IsNullOrEmpty(report.InvoiceNumber) && typeof(T) == typeof(TachographDocument))
            {
                documents = documents.Where(document =>
                {
                    var tachographDocument = document as TachographDocument;
                    if (tachographDocument != null)
                    {
                        return tachographDocument.InvoiceNumber == report.InvoiceNumber;
                    }
                    return false;
                });
            }

            return documents;
        }
    }
}