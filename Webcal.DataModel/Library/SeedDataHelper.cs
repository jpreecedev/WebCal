namespace Webcal.DataModel.Library
{
    using System;
    using Core;
    using Properties;
    using Shared;

    public static class SeedDataHelper
    {
        public static ISettingsRepository<WorkshopSettings> SeedDatabase()
        {
            //This is a bit of a "fudge" to ensure that seed data has been created
            var settings = ContainerBootstrapper.Container.GetInstance<ISettingsRepository<WorkshopSettings>>();
            if (settings.GetWorkshopSettings() == null)
            {
                using (var context = new TachographContext())
                {
                    Seed(context);
                    context.SaveChanges();
                }
            }

            //Check that the 'super user account' exists
            var userRepository = ContainerBootstrapper.Container.GetInstance<IRepository<User>>();
            if (userRepository.FirstOrDefault(user => string.Equals(user.Username, "superuser", StringComparison.CurrentCultureIgnoreCase)) == null)
            {
                UserManagement.AddSuperUser(userRepository);
                userRepository.Save();
            }

            UserManagement.AddDefaultUser();

            return settings;
        }

        private static void Seed(TachographContext context)
        {
            if (context == null)
            {
                return;
            }

            context.WorkshopSettings.Add(new WorkshopSettings
            {
                BackupFilePath = Resources.TXT_NO_PATH_SPECIFIED,
                AutoPrintLabels = true
            });

            context.PrinterSettings.Add(new PrinterSettings
            {
                AlwaysAskForPrinter = true,
                IsPortrait = true,
                DefaultNumberOfCopies = 1,
                LabelNumberOfCopies = 1
            });

            context.RegistrationData.Add(new RegistrationData
            {
                LicenseKey = null,
                CompanyName = "Skillray",
                SealNumber = ""
            });
        }
    }
}