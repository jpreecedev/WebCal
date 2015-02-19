namespace Webcal.Library.PDF
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using Connect.Shared.Models;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using Shared;
    using Shared.Helpers;
    using Shared.Workers;

    public static class PDFExtensions
    {
        public static PDFDocumentResult ToPDF<T>(this T document, bool readOnly = true, bool excludeLogos = false, bool promptUser = false) where T : Document
        {
            if (!readOnly)
                document.InspectionDate = DateTime.Now;

            DialogHelperResult result = Save(promptUser);
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

        public static PDFDocumentResult GenerateVOSADocument(this ICollection<TachographDocument> documents, DateTime start, DateTime end)
        {
            DialogHelperResult result = DialogHelper.SaveFile(DialogFilter.PDF, string.Empty);
            if (result.Result == true)
            {
                VOSADocument.Create(result.FileName, documents, start, end);
                return new PDFDocumentResult { FilePath = result.FileName };
            }
            return new PDFDocumentResult();
        }

        public static void Email(this PDFDocumentResult pdfDocumentResult, WorkshopSettings workshopSettings, MailSettings mailSettings)
        {
            if (pdfDocumentResult.Document == null)
            {
                return;
            }

            EmailHelper.SendEmail(workshopSettings, mailSettings, pdfDocumentResult.Document, pdfDocumentResult.FilePath);
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

            var repository = ContainerBootstrapper.Container.GetInstance<ISettingsRepository<PrinterSettings>>();
            var settings = repository.GetPrinterSettings();

            var workerTask = new WorkerTask { TaskName = WorkerTaskName.Print };

            workerTask.Parameters = new WorkerParameters();
            workerTask.Parameters.SetParameter("FilePath", pdfDocumentResult.FilePath);
            workerTask.Parameters.SetParameter("AlwaysAskForPrinter", settings.AlwaysAskForPrinter);
            workerTask.Parameters.SetParameter("DefaultPrinterName", settings.DefaultPrinterName);
            workerTask.Parameters.SetParameter("DefaultNumberOfCopies", settings.DefaultNumberOfCopies);
            workerTask.Parameters.SetParameter("LabelNumberOfCopies", settings.LabelNumberOfCopies);

            WorkerHelper.RunTask(workerTask);
            return true;
        }

        private static DialogHelperResult Save(bool promptUser)
        {
            if (!promptUser)
            {
                string path = Path.Combine(ImageHelper.GetTemporaryDirectory(), string.Format("{0}.pdf", Guid.NewGuid().ToString().Replace("-", "")));
                return new DialogHelperResult { FileName = path, Result = true };
            }

            DialogHelperResult result = DialogHelper.SaveFile(DialogFilter.PDF, string.Empty);
            return result;
        }
    }
}