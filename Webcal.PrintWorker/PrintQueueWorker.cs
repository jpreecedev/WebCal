namespace Webcal.PrintWorker
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using Shared.Workers;

    public class PrintQueueWorker : BaseWorker
    {
        public PrintQueueWorker(Action<string> sendMessage)
            : base(sendMessage)
        {
        }

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

            string pdfExecutablePath = FindExecutable(printParameters.FilePath);
            if (string.IsNullOrEmpty(pdfExecutablePath))
            {
                SendMessage("Unable to find suitable PDF executable path");
                return;
            }

            try
            {
                SendMessage("Preparing to print");

                var proc = new Process
                {
                    StartInfo =
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Verb = "Print",
                        FileName = pdfExecutablePath,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Arguments = GetStartupArguments(printParameters.FilePath, printParameters)
                    }
                };

                proc.EnableRaisingEvents = true;

                for (int i = 0; i < printParameters.DefaultNumberOfCopies; i++)
                {
                    proc.Start();
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.WaitForExit(180000);
                }

                SendMessage("Print complete, tidying up.");

                proc.Close();
                KillPDFViewer(Path.GetFileNameWithoutExtension(pdfExecutablePath));
            }
            catch (Exception ex)
            {
                SendMessage(ex.Message);
            }
        }

        private static string GetStartupArguments(string filePath, PrintParameters parameters)
        {
            if (parameters.AlwaysAskForPrinter || string.IsNullOrEmpty(parameters.DefaultPrinterName))
            {
                return string.Format(@"/p ""{0}""", filePath);
            }

            return string.Format(@"/t ""{0}"" ""{1}""", filePath, parameters.DefaultPrinterName);
        }

        private static string FindExecutable(string path)
        {
            var executable = new StringBuilder(1024);
            int result = FindExecutable(path, string.Empty, executable);
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