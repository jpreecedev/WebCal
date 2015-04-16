﻿namespace TachographReader.Library
{
    using System;
    using System.Data.SqlServerCe;
    using System.IO;
    using System.Linq;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using Properties;
    using Shared;
    using Shared.Helpers;

    public static class MigrationHelper
    {
        public static void ApplyDataHacks()
        {
            var miscellaneousSettingsRepository = ContainerBootstrapper.Resolve<ISettingsRepository<MiscellaneousSettings>>();
            var miscellaneousSettings = miscellaneousSettingsRepository.GetMiscellaneousSettings();
            var lastMigrationHackId = miscellaneousSettings.LastMigrationHackId;

            if (lastMigrationHackId == 0)
            {
                var workshopSettings = ContainerBootstrapper.Resolve<ISettingsRepository<WorkshopSettings>>().GetWorkshopSettings();
                var printerSettingsRepository = ContainerBootstrapper.Resolve<ISettingsRepository<PrinterSettings>>();

                var printerSettings = printerSettingsRepository.GetPrinterSettings();
                printerSettings.AutoPrintLabels = workshopSettings.AutoPrintLabels;

                printerSettingsRepository.Save(printerSettings);
                miscellaneousSettings.LastMigrationHackId = 1;
                miscellaneousSettingsRepository.Save(miscellaneousSettings);
            }
        }

        public static void MigrateIfRequired()
        {
            MigrateOldPath((string) AppDomain.CurrentDomain.GetData("DataDirectory"));
            MigrateWorkshopImages();
        }

        public static void SetDatabasePermissions()
        {
            var databasePath = (string) AppDomain.CurrentDomain.GetData("DataDirectory");

            if (IsAdministrator())
            {
                GrantAccess(Path.Combine(databasePath, "tacho.sdf"));
            }
        }

        private static void MigrateWorkshopImages()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Webcal", "ContactImages");
            if (Directory.Exists(path))
            {
                var supportedFormats = new[] {Resources.TXT_EXTENSION_JPG, Resources.TXT_EXTENSION_JPEG, Resources.TXT_EXTENSION_PNG};
                var logo = Directory.GetFiles(path).FirstOrDefault(f => supportedFormats.Any(format => string.Equals(Path.GetExtension(f), format, StringComparison.CurrentCultureIgnoreCase)));
                if (logo != null)
                {
                    var repository = ContainerBootstrapper.Resolve<ISettingsRepository<WorkshopSettings>>();
                    try
                    {
                        var workshopSettings = repository.GetWorkshopSettings();
                        workshopSettings.Image = ImageHelper.LoadImageSafely(logo);
                        repository.Save(workshopSettings);
                    }
                    catch
                    {
                    }
                }

                var directoryInfo = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Webcal"));
                directoryInfo.EmptyFolder();
                directoryInfo.Delete(true);
            }
        }

        private static void MigrateOldPath(string databasePath)
        {
            var oldPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (File.Exists(Path.Combine(oldPath, "webcal.sdf")) && !(File.Exists(Path.Combine(databasePath, "webcal.sdf"))))
            {
                File.Move(Path.Combine(oldPath, "webcal.sdf"), Path.Combine(databasePath, "tacho.sdf"));
            }
        }

        private static bool IsAdministrator()
        {
            try
            {
                var user = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(user);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        private static void GrantAccess(string fullPath)
        {
            var dInfo = new DirectoryInfo(fullPath);
            var dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
        }

        public static void HackMigrationHistoryTable()
        {
            var databasePath = Path.Combine((string) AppDomain.CurrentDomain.GetData("DataDirectory"), "tacho.sdf");

            if (!File.Exists(databasePath))
            {
                return;
            }

            using (var connection = new SqlCeConnection(string.Format(@"Data Source = {0}", databasePath)))
            {
                using (var cmd = new SqlCeCommand(@"UPDATE [__MigrationHistory] SET ContextKey = 'TachographReader.DataModel.Migrations.Configuration' WHERE ContextKey = 'Webcal.DataModel.Migrations.Configuration'", connection))
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}