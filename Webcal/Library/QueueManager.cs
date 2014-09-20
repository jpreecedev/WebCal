namespace Webcal.Library
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using DataModel;
    using DataModel.Core;
    using Properties;
    using Shared;

    public static class QueueManager
    {
        public static void BackUp()
        {
            if (!CanBackupOrRestore())
            {
                MessageBoxHelper.ShowError(Resources.EXC_CANNOT_BACKUP_DATABASE);
                return;
            }

            string databasePath = GetDatabasePath();
            if (!File.Exists(databasePath))
            {
                MessageBoxHelper.ShowError(Resources.EXC_UNABLE_VERIFY_DATABASE_PATH);
                return;
            }

            //Backup:   /b <dbpath> <backuppath>
            DialogHelperResult result = DialogHelper.SaveFile(DialogFilter.SQLServerCEDatabaseFile, "");
            if (result.Result == true)
                Run(string.Format(@"/b ""{0}"" ""{1}""", databasePath, result.FileName), false);
        }

        private static void Run(string arguments, bool quiet)
        {
            try
            {
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = "Webcal.Queue.exe";
                startInfo.Arguments = arguments;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;

                var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
                process.Start();
                process.WaitForExit(300000);

                if (!quiet)
                    MessageBoxHelper.ShowMessage(process.StandardOutput.ReadToEnd());
            }
            catch (Exception ex)
            {
                if (!quiet)
                    MessageBoxHelper.ShowError(string.Format("{0}\n\n{1}", Resources.TXT_UNABLE_COMPLETE_RESTORE, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex)));
            }
        }

        private static bool CanBackupOrRestore()
        {
            return string.Equals(ConfigurationManager.ConnectionStrings["TachographContext"].ProviderName, "System.Data.SqlServerCe.4.0", StringComparison.CurrentCultureIgnoreCase);
        }

        private static string GetDatabasePath()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TachographContext"].ConnectionString;
            return connectionString.Replace("DataSource=|DataDirectory|", (string)AppDomain.CurrentDomain.GetData("DataDirectory") + "\\");
        }

        private static bool HasBackupBeenTakenToday(string backupPath)
        {
            string[] files = Directory.GetFiles(backupPath, "*.sdf");

            foreach (string file in files)
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    if (fileName != null && fileName.Contains("_"))
                    {
                        string datePart = fileName.Split(Char.Parse("_"))[1].Substring(0, 8);
                        int day = datePart.Substring(0, 2).ToInt();
                        int month = datePart.Substring(2, 2).ToInt();
                        int year = datePart.Substring(4, 4).ToInt();

                        if (day == DateTime.Now.Day && month == DateTime.Now.Month && year == DateTime.Now.Year)
                            return true;
                    }
                }
                catch
                {
                    //Filename does not follow naming format, so its invalid
                }
            }

            return false;
        }
    }
}