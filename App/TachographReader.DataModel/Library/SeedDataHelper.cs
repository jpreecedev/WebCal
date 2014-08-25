﻿namespace Webcal.DataModel.Library
{
    using System;
    using Core;
    using Properties;
    using Repositories;
    using StructureMap;

    public static class SeedDataHelper
    {
        public static IGeneralSettingsRepository SeedDatabase()
        {
            //This is a bit of a "fudge" to ensure that seed data has been created
            var generalSettings = ObjectFactory.GetInstance<IGeneralSettingsRepository>();
            if (generalSettings.GetSettings() == null)
            {
                using (var context = new TachographContext())
                {
                    Seed(context);
                    context.SaveChanges();
                }
            }

            //Check that the 'super user account' exists
            var userRepository = ObjectFactory.GetInstance<UserRepository>();
            if (userRepository.FirstOrDefault(user => string.Equals(user.Username, "superuser", StringComparison.CurrentCultureIgnoreCase)) == null)
            {
                UserManagement.AddSuperUser(userRepository);
                userRepository.Save();
            }

            return generalSettings;
        }

        public static void Seed(TachographContext context)
        {
            if (context == null)
                return;

            context.WorkshopSettings.Add(new WorkshopSettings
            {
                BackupFilePath = Resources.TXT_NO_PATH_SPECIFIED,
                AutoPrintLabels = true
            });

            context.PrinterSettings.Add(new PrinterSettings
            {
                AlwaysAskForPrinter = true,
                IsPortrait = true
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