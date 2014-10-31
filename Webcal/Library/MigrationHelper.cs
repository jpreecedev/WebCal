﻿namespace Webcal.Library
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.Windows.Media.Imaging;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using Shared;

    public static class MigrationHelper
    {
        public static void MigrateIfRequired()
        {
            MigrateOldPath((string)AppDomain.CurrentDomain.GetData("DataDirectory"));
            MigrateWorkshopImages();
        }

        public static void SetDatabasePermissions()
        {
            var databasePath = (string)AppDomain.CurrentDomain.GetData("DataDirectory");

            if (IsAdministrator())
            {
                GrantAccess(Path.Combine(databasePath, "webcal.sdf"));
            }
        }

        private static void MigrateWorkshopImages()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Webcal", "ContactImages");
            if (Directory.Exists(path))
            {
                var supportedFormats = new[] { ".jpg", ".jpeg", ".png" };
                var logo = Directory.GetFiles(path).FirstOrDefault(f => supportedFormats.Any(format => string.Equals(Path.GetExtension(f), format, StringComparison.CurrentCultureIgnoreCase)));
                if (logo != null)
                {
                    var repository = ContainerBootstrapper.Container.GetInstance<ISettingsRepository<WorkshopSettings>>();
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

                DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Webcal"));
                directoryInfo.EmptyFolder();
                directoryInfo.Delete(true);
            }
        }

        private static void MigrateOldPath(string databasePath)
        {
            var oldPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (File.Exists(Path.Combine(oldPath, "webcal.sdf")) && !(File.Exists(Path.Combine(databasePath, "webcal.sdf"))))
            {
                File.Move(Path.Combine(oldPath, "webcal.sdf"), Path.Combine(databasePath, "webcal.sdf"));
            }
        }

        private static bool IsAdministrator()
        {
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        private static void GrantAccess(string fullPath)
        {
            DirectoryInfo dInfo = new DirectoryInfo(fullPath);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
        }
    }
}