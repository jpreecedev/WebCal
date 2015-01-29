namespace Webcal.Library.PDF
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using Shared;
    using Shared.Workers;

    public static class PDFHelper
    {
        public static string LastPDFOutputPath = string.Empty;

        public static bool GenerateTachographPlaque(Document document, bool saveToTempDirectory, bool isHistoryMode, bool excludeLogos)
        {
            if (document == null)
            {
                return false;
            }

            if (!isHistoryMode)
            {
                document.InspectionDate = DateTime.Now;
            }

            LetterForDecommissioningDocument letterForDecommissioningDocument = document as LetterForDecommissioningDocument;
            if (letterForDecommissioningDocument != null)
                return GenerateTachographPlaque(letterForDecommissioningDocument, saveToTempDirectory);

            var undownloadabilityDocument = document as UndownloadabilityDocument;
            if (undownloadabilityDocument != null)
            {
                return GenerateTachographPlaque(undownloadabilityDocument, saveToTempDirectory);
            }

            var tachographDocument = document as TachographDocument;
            if (tachographDocument != null)
            {
                return GenerateTachographPlaque(tachographDocument, saveToTempDirectory, excludeLogos);
            }

            return false;
        }

        public static bool GenerateTachographPlaque(TachographDocument document, bool saveToTempDirectory, bool excludeLogos)
        {
            if (document == null)
            {
                return false;
            }

            DialogHelperResult result = Save(saveToTempDirectory);
            if (result.Result == true)
            {
                using (var pdfDocument = new PDFDocument(result.FileName))
                {
                    IPlaque plaque = document.TachographHasAdapter ? (IPlaque) new FullPlaqueDocument() : new MinimalPlaqueDocument();
                    plaque.CreateFullCertificate(pdfDocument, document, excludeLogos);
                }

                return true;
            }

            return false;
        }

        public static bool GenerateTachographPlaque(UndownloadabilityDocument document, bool saveToTempDirectory)
        {
            if (document == null)
            {
                return false;
            }

            DialogHelperResult result = Save(saveToTempDirectory);
            if (result.Result == true)
            {
                using (var pdfDocument = new PDFDocument(result.FileName))
                {
                    UndownloadabilityCertificate.Create(pdfDocument, document);
                }

                return true;
            }

            return false;
        }

        public static bool GenerateTachographPlaque(LetterForDecommissioningDocument document, bool saveToTempDirectory)
        {
            if (document == null)
                return false;

            DialogHelperResult result = Save(saveToTempDirectory);
            if (result.Result == true)
            {
                using (PDFDocument pdfDocument = new PDFDocument(result.FileName))
                {
                    LetterForDecommissioning.Create(pdfDocument, document);
                }

                return true;
            }

            return false;
        }

        public static void GenerateVOSADocument(List<TachographDocument> documents, DateTime start, DateTime end)
        {
            DialogHelperResult result = DialogHelper.SaveFile(DialogFilter.PDF, string.Empty);
            if (result.Result == true)
            {
                VOSADocument.Create(result.FileName, documents, start, end);
            }
        }

        public static void Print(string path)
        {
            var repository = ContainerBootstrapper.Container.GetInstance<ISettingsRepository<PrinterSettings>>();
            var settings = repository.GetPrinterSettings();

            var workerTask = new WorkerTask {TaskName = WorkerTaskName.Print};

            workerTask.Parameters = new WorkerParameters();
            workerTask.Parameters.SetParameter("FilePath", path);
            workerTask.Parameters.SetParameter("AlwaysAskForPrinter", settings.AlwaysAskForPrinter);
            workerTask.Parameters.SetParameter("DefaultPrinterName", settings.DefaultPrinterName);
            workerTask.Parameters.SetParameter("DefaultNumberOfCopies", settings.DefaultNumberOfCopies);
            workerTask.Parameters.SetParameter("LabelNumberOfCopies", settings.LabelNumberOfCopies);

            WorkerHelper.RunTask(workerTask);
        }

        private static DialogHelperResult Save(bool saveToTempDirectory)
        {
            if (saveToTempDirectory)
            {
                string path = Path.Combine(ImageHelper.GetTemporaryDirectory(), string.Format("document.pdf"));
                return new DialogHelperResult {FileName = path, Result = true};
            }

            DialogHelperResult result = DialogHelper.SaveFile(DialogFilter.PDF, string.Empty);
            LastPDFOutputPath = result.FileName;

            return result;
        }
    }
}