namespace Webcal.Library.PDF
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using DataModel;
    using DataModel.Core;
    using Properties;
    using Shared;
    using StructureMap;

    public static class PDFHelper
    {
        public static string LastPDFOutputPath = string.Empty;

        [DllImport("shell32.dll")]
        private static extern int FindExecutable(string lpFile, string lpDirectory, [Out] StringBuilder lpResult);

        public static bool GenerateTachographPlaque(Document document, bool saveToTempDirectory)
        {
            if (document == null)
                return false;

            document.InspectionDate = DateTime.Now;

            var undownloadabilityDocument = document as UndownloadabilityDocument;
            if (undownloadabilityDocument != null)
                return GenerateTachographPlaque(undownloadabilityDocument, saveToTempDirectory);

            var tachographDocument = document as TachographDocument;
            if (tachographDocument != null)
                return GenerateTachographPlaque(tachographDocument, saveToTempDirectory);

            return false;
        }

        public static bool GenerateTachographPlaque(TachographDocument document, bool saveToTempDirectory)
        {
            if (document == null)
                return false;

            DialogHelperResult result = Save(saveToTempDirectory);
            if (result.Result == true)
            {
                using (var pdfDocument = new PDFDocument(result.FileName))
                {
                    IPlaque plaque = document.TachographHasAdapter ? (IPlaque) new FullPlaqueDocument() : new MinimalPlaqueDocument();
                    plaque.CreateFullCertificate(pdfDocument, document);
                    //CreateFullCertificate  Create
                }

                return true;
            }

            return false;
        }

        public static bool GenerateTachographPlaque(UndownloadabilityDocument document, bool saveToTempDirectory)
        {
            if (document == null)
                return false;

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

        public static void GenerateVOSADocument(List<TachographDocument> documents, DateTime start, DateTime end)
        {
            DialogHelperResult result = DialogHelper.SaveFile(DialogFilter.PDF, "");
            if (result.Result == true)
                VOSADocument.Create(result.FileName, documents, start, end);
        }

        public static bool Print(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            string pdfExecutablePath = FindExecutable(filePath);
            if (string.IsNullOrEmpty(pdfExecutablePath))
            {
                MessageBoxHelper.ShowError(Resources.EXC_CANNOT_PRINT_PDF);
                return false;
            }

            try
            {
                var proc = new Process();
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.Verb = "Print";
                proc.StartInfo.FileName = pdfExecutablePath;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.Arguments = GetStartupArguments(filePath);

                proc.Start();
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.WaitForExit(10000);

                proc.EnableRaisingEvents = true;

                proc.Close();
                KillPDFViewer(Path.GetFileNameWithoutExtension(pdfExecutablePath));
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError(string.Format("{0}\n\n{1}", Resources.EXC_UNABLE_PRINT_PDF, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex)));
                return false;
            }

            return true;
        }

        private static string GetStartupArguments(string filePath)
        {
            var repository = ContainerBootstrapper.Container.GetInstance<IPrinterSettingsRepository>();
            PrinterSettings settings = repository.GetSettings();

            if (settings.AlwaysAskForPrinter || string.IsNullOrEmpty(settings.DefaultPrinterName))
                return string.Format(@"/p ""{0}""", filePath);


            return string.Format(@"/t ""{0}"" ""{1}""", filePath, settings.DefaultPrinterName);
        }

        private static string FindExecutable(string path)
        {
            var executable = new StringBuilder(1024);
            int result = FindExecutable(path, string.Empty, executable);
            return result >= 32 ? executable.ToString() : string.Empty;
        }

        private static void KillPDFViewer(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses().Where(clsProcess => clsProcess.ProcessName.StartsWith(name)))
            {
                try
                {
                    clsProcess.Kill();
                }
                catch
                {
                }
            }
        }

        private static DialogHelperResult Save(bool saveToTempDirectory)
        {
            if (saveToTempDirectory)
            {
                string path = Path.Combine(DocumentHelper.GetTemporaryDirectory(), "document.pdf");
                return new DialogHelperResult {FileName = path, Result = true};
            }

            DialogHelperResult result = DialogHelper.SaveFile(DialogFilter.PDF, string.Empty);
            LastPDFOutputPath = result.FileName;

            return result;
        }
    }
}