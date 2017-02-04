namespace TachographReader.Library
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Connect.Shared;
    using DataModel.Core;
    using Properties;
    using Shared;
    using Shared.Helpers;

    public static class BackupRestoreManager
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
            DialogHelperResult result = DialogHelper.SaveFile(DialogFilter.SQLServerCEDatabaseFile, string.Empty);
            if (result.Result == true)
            {
                Run($@"/b ""{databasePath}"" ""{result.FileName}""", false);
            }
        }

        public static void Restore()
        {
            if (!CanBackupOrRestore())
            {
                MessageBoxHelper.ShowError(Resources.EXC_CANNOT_BACKUP_DATABASE);
                return;
            }

            string databasePath = GetDatabasePath();

            //Restore:  /r <dbpath> <restorepath> <processname>
            DialogHelperResult result = DialogHelper.OpenFile(DialogFilter.SQLServerCEDatabaseFile, string.Empty);
            if (result.Result == true)
            {
                Run($@"/r ""{result.FileName}"" ""{databasePath}"" ""{Assembly.GetExecutingAssembly().GetName().CodeBase}""", false);
            }
        }

        public static void BackupIfRequired(WorkshopSettings workshopSettings)
        {
            if (workshopSettings == null)
            {
                throw new ArgumentNullException(nameof(workshopSettings));
            }

            if (workshopSettings.BackupFilePath == null)
            {
                return;
            }

            if (workshopSettings.BackupFilePath.Length < 3 || workshopSettings.BackupFilePath == Resources.TXT_NO_PATH_SPECIFIED || workshopSettings.AutoBackup == false || workshopSettings.CustomDayOfWeeks == null || workshopSettings.CustomDayOfWeeks.Count == 0)
            {
                return;
            }

            if (workshopSettings.CustomDayOfWeeks.Any(d => CustomDayOfWeek.Parse(d.DayOfWeek) == DateTime.Now.DayOfWeek))
            {
                AutoBackup(workshopSettings.BackupFilePath);
            }
        }

        public static void AutoBackup(string backupPath)
        {
            if (string.IsNullOrEmpty(backupPath) || !Directory.Exists(backupPath))
            {
                return;
            }

            if (HasBackupBeenTakenToday(backupPath))
            {
                return;
            }

            if (!CanBackupOrRestore() || string.Equals(Resources.TXT_NO_PATH_SPECIFIED, backupPath))
            {
                return;
            }

            string databasePath = GetDatabasePath();
            if (!File.Exists(databasePath))
            {
                return;
            }

            string fileName = $"{backupPath}\\{Path.GetFileNameWithoutExtension(databasePath)}_{DateTime.Now.ToString("ddMMyyyyHHmmss")}{Path.GetExtension(databasePath)}";

            //Backup:   /b <dbpath> <backuppath>
            Run($@"/b ""{databasePath}"" ""{fileName}""", true);
        }

        private static void Run(string arguments, bool quiet)
        {
            try
            {
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = "BackupRestoreUtility.exe";
                startInfo.Arguments = arguments;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                startInfo.RedirectStandardOutput = true;

                var process = new Process {StartInfo = startInfo, EnableRaisingEvents = true};
                process.Start();
                process.WaitForExit(300000);

                if (!quiet)
                {
                    MessageBoxHelper.ShowMessage(process.StandardOutput.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                if (!quiet)
                {
                    MessageBoxHelper.ShowError($"{Resources.TXT_UNABLE_COMPLETE_RESTORE}\n\n{ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex)}");
                }
            }
        }

        private static bool CanBackupOrRestore()
        {
            return string.Equals(ConfigurationManager.ConnectionStrings["TachographContext"].ProviderName, "System.Data.SqlServerCe.4.0", StringComparison.CurrentCultureIgnoreCase);
        }

        private static string GetDatabasePath()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["TachographContext"].ConnectionString;
            return connectionString.Replace("DataSource=|DataDirectory|", (string) AppDomain.CurrentDomain.GetData("DataDirectory") + "\\").Replace(";Max Database Size=4091", string.Empty);
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
                        {
                            return true;
                        }
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