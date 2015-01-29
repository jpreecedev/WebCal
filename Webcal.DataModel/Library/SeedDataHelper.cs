namespace Webcal.DataModel.Library
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
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

            var themeSettingsRepository = ContainerBootstrapper.Container.GetInstance<ISettingsRepository<ThemeSettings>>();
            if (themeSettingsRepository.GetThemeSettings() == null)
            {
                using (var context = new TachographContext())
                {
                    context.ThemeSettings.Add(new ThemeSettings { SelectedTheme = "Silver" });
                    context.SaveChanges();
                }
            }

            var miscellaneousSettingsRepository = ContainerBootstrapper.Container.GetInstance<ISettingsRepository<MiscellaneousSettings>>();
            if (miscellaneousSettingsRepository.GetMiscellaneousSettings() == null)
            {
                using (var context = new TachographContext())
                {
                    context.MiscellaneousSettings.Add(new MiscellaneousSettings());
                    context.SaveChanges();
                }
            }

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
                CompanyName = Resources.TXT_SEED_SKILLRAY,
                SealNumber = string.Empty
            });

            context.MailSettings.Add(new MailSettings
            {
                AutoEmailCertificates = false,
                PersonaliseMyEmails = false
            });

            context.TachographMakes.AddRange(new[]
            {
                new TachographMake
                {
                    Id = 1,
                    Name = Resources.TXT_SEED_SIEMENS_VDO,
                    Models = new ObservableCollection<TachographModel>
                    {
                        new TachographModel { Name=Resources.TXT_SEED_TACHO_MODEL_NAME }
                    }
                },
                new TachographMake
                {
                    Id = 2,
                    Name=Resources.TXT_SEED_TACHO_MAKE_NAME,
                    Models = new ObservableCollection<TachographModel>
                    {
                        new TachographModel { Name=Resources.TXT_SEED_TACHO_MAKE_2_MODEL },
                        new TachographModel { Name=Resources.TXT_SEED_TACHO_MAKE_2_MODEL_2 },
                        new TachographModel { Name=Resources.TXT_SEED_TACHO_MAKE_2_MODEL_3 },
                        new TachographModel { Name=Resources.TXT_SEED_TACHO_MAKE_2_MODEL_4 }
                    }
                },
                new TachographMake
                {
                    Id = 3,
                    Name=Resources.TXT_SEED_TACHO_MAKE_3_NAME,
                    Models = new ObservableCollection<TachographModel>
                    {
                        new TachographModel { Name=Resources.TXT_SEED_TACHO_MAKE_3_MODEL_1_NAME },
                        new TachographModel { Name=Resources.TXT_SEED_TACHO_MAKE_3_MODEL_2_NAME },
                        new TachographModel { Name=Resources.TXT_SEED_TACHO_MAKE_3_MODEL_3_NAME }
                    }
                }
            });

            context.VehicleMakes.AddRange(new[]
            {
                new VehicleMake
                {
                    Id = 1,
                    Name = Resources.TXT_SEED_VEHICLE_MAKE_1_NAME,
                    Models = new ObservableCollection<VehicleModel>
                    {
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_MAKE_MODEL_1_NAME},
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_MAKE_MODEL_2_NAME},
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_MAKE_MODEL_3_NAME}
                    }
                },
                new VehicleMake
                {
                    Id = 2,
                    Name = Resources.TXT_SEED_VEHICLE_MAKE_2_NAME,
                    Models = new ObservableCollection<VehicleModel>
                    {
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_2_MAKE_1_NAME},
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_2_MAKE_2_NAME}
                    }
                },
                new VehicleMake
                {
                    Id = 3,
                    Name = Resources.TXT_SEED_VEHICLE_MAKE_3_NAME,
                    Models = new ObservableCollection<VehicleModel>
                    {
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_3_MAKE_1_NAME},
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_3_MAKE_2_NAME},
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_3_MAKE_3_NAME},
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_3_MAKE_4_NAME}
                    }
                },
                new VehicleMake
                {
                    Id = 4,
                    Name = Resources.TXT_SEED_VEHICLE_MAKE_4_NAME,
                    Models = new ObservableCollection<VehicleModel>
                    {
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_4_MAKE_1_NAME},
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_4_MAKE_2_NAME}
                    }
                },
                new VehicleMake
                {
                    Id = 5,
                    Name = Resources.TXT_SEED_VEHICLE_MAKE_5_NAME,
                    Models = new ObservableCollection<VehicleModel>
                    {
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_5_MAKE_1_NAME},
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_5_MAKE_2_NAME},
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_5_MAKE_3_NAME}
                    }
                },
                new VehicleMake
                {
                    Id = 6,
                    Name = Resources.TXT_SEED_VEHICLE_MAKE_6_NAME,
                    Models = new ObservableCollection<VehicleModel>
                    {
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_6_MAKE_1_NAME},
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_6_MAKE_2_NAME},
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_6_MAKE_3_NAME}
                    }
                },
                new VehicleMake
                {
                    Id = 7,
                    Name = Resources.TXT_SEED_VEHICLE_MAKE_7_NAME,
                    Models = new ObservableCollection<VehicleModel>
                    {
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_7_MAKE_1_NAME},
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_7_MAKE_2_NAME}
                    }
                },
                new VehicleMake
                {
                    Id = 8,
                    Name = Resources.TXT_SEED_VEHICLE_MAKE_8_NAME,
                    Models = new ObservableCollection<VehicleModel>
                    {
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_8_MAKE_1_NAME},
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_8_MAKE_2_NAME},
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_8_MAKE_3_NAME}
                    }
                },
                new VehicleMake
                {
                    Id = 9,
                    Name = Resources.TXT_SEED_VEHICLE_MAKE_9_NAME,
                    Models = new ObservableCollection<VehicleModel>
                    {
                        new VehicleModel{ Name = Resources.TXT_SEED_VEHICLE_9_MAKE_1_NAME},
                    }
                },
                new VehicleMake
                {
                    Id = 10,
                    Name = Resources.TXT_SEED_VEHICLE_10_MAKE_1_NAME,
                }
            });
        }
    }
}