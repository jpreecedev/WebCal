namespace Webcal.DataModel.Library
{
    using System;
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

            context.TachographMakes.AddRange(new[]
            {
                new TachographMake
                {
                    Id = 1,
                    Name = "SIEMENS VDO",
                    Models = new ObservableCollection<TachographModel>
                    {
                        new TachographModel { Name="1381" }
                    }
                },
                new TachographMake
                {
                    Id = 2,
                    Name="VDO",
                    Models = new ObservableCollection<TachographModel>
                    {
                        new TachographModel { Name="1318" },
                        new TachographModel { Name="1324" },
                        new TachographModel { Name="1314" },
                        new TachographModel { Name="1319" }
                    }
                },
                new TachographMake
                {
                    Id = 3,
                    Name="STONERIDGE",
                    Models = new ObservableCollection<TachographModel>
                    {
                        new TachographModel { Name="SE5000" },
                        new TachographModel { Name="8400" },
                        new TachographModel { Name="2400" }
                    }
                }
            });

            context.VehicleMakes.AddRange(new[]
            {
                new VehicleMake
                {
                    Id = 1,
                    Name = "DAF",
                    Models = new ObservableCollection<VehicleModel>
                    {
                        new VehicleModel{ Name = "LF"},
                        new VehicleModel{ Name = "CF"},
                        new VehicleModel{ Name = "XF"}
                    }
                },
                new VehicleMake
                {
                    Id = 2,
                    Name = "SCANIA",
                    Models = new ObservableCollection<VehicleModel>
                    {
                        new VehicleModel{ Name = "R SERIES"},
                        new VehicleModel{ Name = "G SERIES"}
                    }
                },
                new VehicleMake
                {
                    Id = 3,
                    Name = "MERCEDES",
                    Models = new ObservableCollection<VehicleModel>
                    {
                        new VehicleModel{ Name = "SPRINTER"},
                        new VehicleModel{ Name = "ATEGO"},
                        new VehicleModel{ Name = "AXOR"},
                        new VehicleModel{ Name = "ACTROS"}
                    }
                },
                new VehicleMake
                {
                    Id = 4,
                    Name = "RENAULT",
                    Models = new ObservableCollection<VehicleModel>
                    {
                        new VehicleModel{ Name = "PREMIUM"},
                        new VehicleModel{ Name = "MAGNUM"}
                    }
                },
                new VehicleMake
                {
                    Id = 5,
                    Name = "VOLVO",
                    Models = new ObservableCollection<VehicleModel>
                    {
                        new VehicleModel{ Name = "FL"},
                        new VehicleModel{ Name = "FH"},
                        new VehicleModel{ Name = "FM"}
                    }
                },
                new VehicleMake
                {
                    Id = 6,
                    Name = "INVECO",
                    Models = new ObservableCollection<VehicleModel>
                    {
                        new VehicleModel{ Name = "STRALIS"},
                        new VehicleModel{ Name = "DAILY"},
                        new VehicleModel{ Name = "EUROCARGO"}
                    }
                },
                new VehicleMake
                {
                    Id = 7,
                    Name = "MAN",
                    Models = new ObservableCollection<VehicleModel>
                    {
                        new VehicleModel{ Name = "TGL"},
                        new VehicleModel{ Name = "TGX"}
                    }
                },
                new VehicleMake
                {
                    Id = 8,
                    Name = "ISUZU",
                    Models = new ObservableCollection<VehicleModel>
                    {
                        new VehicleModel{ Name = "D MAX"},
                        new VehicleModel{ Name = "N35"},
                        new VehicleModel{ Name = "N75"}
                    }
                },
                new VehicleMake
                {
                    Id = 9,
                    Name = "DENNIS",
                    Models = new ObservableCollection<VehicleModel>
                    {
                        new VehicleModel{ Name = "EAGLE"},
                    }
                },
                new VehicleMake
                {
                    Id = 10,
                    Name = "HINO",
                }
            });
        }
    }
}