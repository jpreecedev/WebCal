namespace TachographReader.Library.PDF
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Connect.Shared;
    using Connect.Shared.Models;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using Shared;
    using Shared.Helpers;
    using Shared.Workers;
    using ViewModels;

    public static class PDFExtensions
    {
        public static PDFDocumentResult ToPDF(this Report report)
        {
            DialogHelperResult result = Save(true);
            if (result.Result == true)
            {
                using (var pdfDocument = new PDFDocument(result.FileName))
                {
                    CustomReport.Create(pdfDocument, report);
                    return new PDFDocumentResult { FilePath = result.FileName };
                }
            }
            return new PDFDocumentResult();
        }

        public static PDFDocumentResult ToReportPDF<T>(this T report, bool readOnly = true, bool excludeLogos = false, bool promptUser = false) where T : BaseReport
        {
            if (!readOnly)
                report.Created = DateTime.Now;

            var result = Save(promptUser);
            if (result.Result == true)
            {
                using (var pdfDocument = new PDFDocument(result.FileName))
                {
                    var qcReportViewModel = report as QCReportViewModel;
                    if (qcReportViewModel != null)
                    {
                        QCCheckReport.Create(pdfDocument, qcReportViewModel);
                    }

                    var qcReport6Month = report as QCReport6Month;
                    if (qcReport6Month != null)
                    {
                        QC6MonthReport.Create(pdfDocument, qcReport6Month);
                    }
                }

                report.SerializedData = File.ReadAllBytes(result.FileName);
                return new PDFDocumentResult { FilePath = result.FileName, Report = report };
            }

            return new PDFDocumentResult();
        }

        public static PDFDocumentResult ToPDF<T>(this T document, bool readOnly = true, bool excludeLogos = false, bool promptUser = false) where T : Document
        {
            if (!readOnly)
                document.InspectionDate = DateTime.Now;

            var result = Save(promptUser);
            if (result.Result == true)
            {
                using (var pdfDocument = new PDFDocument(result.FileName))
                {
                    var tachographDocument = document as TachographDocument;
                    if (tachographDocument != null)
                    {
                        IPlaque plaque = tachographDocument.TachographHasAdapter ? (IPlaque)new FullPlaqueDocument() : new MinimalPlaqueDocument();
                        plaque.CreateFullCertificate(pdfDocument, tachographDocument, excludeLogos);
                    }

                    var undownloadabilityDocument = document as UndownloadabilityDocument;
                    if (undownloadabilityDocument != null)
                    {
                        UndownloadabilityCertificate.Create(pdfDocument, undownloadabilityDocument);
                    }

                    var letterForDecommissioningDocument = document as LetterForDecommissioningDocument;
                    if (letterForDecommissioningDocument != null)
                    {
                        LetterForDecommissioning.Create(pdfDocument, letterForDecommissioningDocument);
                    }
                }

                document.SerializedData = File.ReadAllBytes(result.FileName);
                return new PDFDocumentResult { FilePath = result.FileName, Document = document };
            }

            return new PDFDocumentResult();
        }
        
        public static PDFDocumentResult GenerateGV212Document(this ICollection<TachographDocument> documents, DateTime start, DateTime end)
        {
            var result = DialogHelper.SaveFile(DialogFilter.PDF, string.Empty);
            if (result.Result == true)
            {
                GV212Document.Create(result.FileName, documents, start, end);
                return new PDFDocumentResult { FilePath = result.FileName };
            }
            return new PDFDocumentResult();
        }

        public static void GenerateStatusReport(this StatusReportViewModel statusReport)
        {
            var path = Web.StatusReport.Create(statusReport);
            Process.Start(path);
        }

        public static void Email(this PDFDocumentResult pdfDocumentResult, WorkshopSettings workshopSettings, MailSettings mailSettings)
        {
            if (pdfDocumentResult.Document == null)
            {
                return;
            }

            EmailHelper.SendEmail(workshopSettings, mailSettings, pdfDocumentResult.Document, pdfDocumentResult.FilePath);
        }

        public static void Open(this PDFDocumentResult pdfDocumentResult)
        {
            try
            {
                Process.Start(pdfDocumentResult.FilePath);
            }
            catch
            {
            }
        }

        public static bool Print(this PDFDocumentResult pdfDocumentResult)
        {
            if (pdfDocumentResult == null)
            {
                return false;
            }
            if (!File.Exists(pdfDocumentResult.FilePath))
            {
                return false;
            }
            if (!pdfDocumentResult.Success)
            {
                return false;
            }

            var repository = ContainerBootstrapper.Resolve<ISettingsRepository<PrinterSettings>>();
            var settings = repository.GetPrinterSettings();

            var parameters = new WorkerParameters();
            parameters.SetParameter("FilePath", pdfDocumentResult.FilePath);
            parameters.SetParameter("AlwaysAskForPrinter", settings.AlwaysAskForPrinter);
            parameters.SetParameter("DefaultPrinterName", settings.DefaultPrinterName);
            parameters.SetParameter("DefaultNumberOfCopies", settings.DefaultNumberOfCopies);
            parameters.SetParameter("LabelNumberOfCopies", settings.LabelNumberOfCopies);
            parameters.SetParameter("AutoClosePDFProgram", settings.AutoClosePDFProgram);
            parameters.SetParameter("ShowCompanyNameOnLabels", settings.ShowCompanyNameOnLabels);
            parameters.SetParameter("Timeout", settings.Timeout);

            var workerTask = new WorkerTask
            {
                TaskName = WorkerTaskName.Print
            };
            workerTask.SetWorkerParameters(parameters);

            WorkerHelper.QueueTask(workerTask);

            return true;
        }

        public static void Email(this GV212Report gv212Report, WorkshopSettings workshopSettings, MailSettings mailSettings)
        {
            ReCreateGV212(gv212Report).Email(workshopSettings, mailSettings);
        }

        public static void Print(this GV212Report gv212Report)
        {
            ReCreateGV212(gv212Report).Print();
        }

        private static PDFDocumentResult ReCreateGV212(GV212Report gv212Report)
        {
            if (gv212Report == null)
            {
                return new PDFDocumentResult();
            }

            var result = Save(false);
            if (result.Result == true)
            {
                File.WriteAllBytes(result.FileName, gv212Report.SerializedData);
                return new PDFDocumentResult
                {
                    FilePath = result.FileName,
                    GV212Report = gv212Report
                };
            }

            return new PDFDocumentResult();
        }

        private static DialogHelperResult Save(bool promptUser)
        {
            if (!promptUser)
            {
                var path = Path.Combine(ImageHelper.GetTemporaryDirectory(), $"{Guid.NewGuid().ToString().Replace("-", "")}.pdf");
                return new DialogHelperResult { FileName = path, Result = true };
            }

            return DialogHelper.SaveFile(DialogFilter.PDF, string.Empty);
        }
    }
}