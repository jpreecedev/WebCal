namespace TachographReader.Shared.Workers.PrintWorker
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using Properties;

    public class PrintQueueWorker : BaseWorker
    {
        [DllImport("shell32.dll")]
        private static extern int FindExecutable(string lpFile, string lpDirectory, [Out] StringBuilder lpResult);

        public override void Start(IWorkerParameters parameters)
        {
            if (parameters == null)
            {
                return;
            }

            var printParameters = new PrintParameters(parameters);
            if (string.IsNullOrEmpty(printParameters.FilePath))
            {
                return;
            }

            var pdfExecutablePath = FindExecutable(printParameters.FilePath);
            if (string.IsNullOrEmpty(pdfExecutablePath))
            {
                throw new Exception(Resources.ERR_UNABLE_FIND_SUITABLE_PDF_EXECUTABLE_PATH);
            }

            for (var i = 0; i < printParameters.DefaultNumberOfCopies; i++)
            {
                var proc = new Process
                {
                    StartInfo =
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Verb = "Print",
                        FileName = pdfExecutablePath,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Arguments = GetStartupArguments(printParameters.FilePath, printParameters, pdfExecutablePath)
                    },
                    EnableRaisingEvents = true
                };

                proc.Start();

                if (printParameters.Timeout > 0)
                {
                    proc.WaitForExit(printParameters.Timeout*1000);
                }
                if (printParameters.AutoClosePDFProgram && !printParameters.AlwaysAskForPrinter)
                {
                    proc.Close();
                    KillPDFViewer(Path.GetFileNameWithoutExtension(pdfExecutablePath));
                }
            }
        }

        private static string GetStartupArguments(string filePath, PrintParameters parameters, string pdfExecutablePath)
        {
            if (pdfExecutablePath != null && pdfExecutablePath.ToUpper().Contains("ADOBE"))
            {
                if (parameters.AlwaysAskForPrinter || string.IsNullOrEmpty(parameters.DefaultPrinterName))
                {
                    return $@"/p /n ""{filePath}""";
                }

                return $@"/t /n ""{filePath}"" ""{parameters.DefaultPrinterName}""";
            }

            if (parameters.AlwaysAskForPrinter || string.IsNullOrEmpty(parameters.DefaultPrinterName))
            {
                return $@"/p ""{filePath}""";
            }

            return $@"/t ""{filePath}"" ""{parameters.DefaultPrinterName}""";
        }

        private static string FindExecutable(string path)
        {
            var executable = new StringBuilder(1024);
            var result = FindExecutable(path, string.Empty, executable);
            return result >= 32 ? executable.ToString() : string.Empty;
        }

        private static void KillPDFViewer(string name)
        {
            foreach (var clsProcess in Process.GetProcesses().Where(clsProcess => clsProcess.ProcessName.StartsWith(name)))
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
    }
}