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
            var superUser = userRepository.FirstOrDefault(user => string.Equals(user.Username, "superuser", StringComparison.CurrentCultureIgnoreCase));

            if (superUser == null)
            {
                UserManagement.AddSuperUser(userRepository);
                userRepository.Save();
            }
            else
            {
                UserManagement.UpdateSuperUser(userRepository, superUser);
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

            context.MailSettings.Add(new MailSettings
            {
                AutoEmailCertificates = false,
                PersonaliseMyEmails = false
            });
        }
    }
}