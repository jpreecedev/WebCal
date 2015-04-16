﻿namespace TachographReader.DataModel.Core
{
    using System;
    using Connect.Shared.Models;
    using ConnectClient;
    using Microsoft.Practices.Unity;
    using Properties;
    using Repositories;
    using Shared;
    using Shared.Connect;
    using Shared.Helpers;
    using Shared.Models;

    public static class ContainerBootstrapper
    {
        private static readonly IUnityContainer _container;

        static ContainerBootstrapper()
        {
            _container = new UnityContainer();
            Configure(_container);
        }

        public static IUnityContainer Container
        {
            get { return _container; }
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        private static void Configure(IUnityContainer container)
        {
            try
            {
                container.RegisterType<IRepository<VehicleMake>, VehicleRepository>();
                container.RegisterType<IRepository<TachographDocument>,TachographDocumentRepository>();
                container.RegisterType<IRepository<TachographMake>,TachographMakesRepository>();
                container.RegisterType<IRepository<Technician>,TechnicianRepository>();

                container.RegisterType<IRepository<UndownloadabilityDocument>,Repository<UndownloadabilityDocument>>();
                container.RegisterType<IRepository<LetterForDecommissioningDocument>,Repository<LetterForDecommissioningDocument>>();
                container.RegisterType<IRepository<CustomerContact>,Repository<CustomerContact>>();
                container.RegisterType<IRepository<TachographFile>,Repository<TachographFile>>();
                container.RegisterType<IRepository<DriverCardFile>,Repository<DriverCardFile>>();
                container.RegisterType<IRepository<WorkshopCardFile>,Repository<WorkshopCardFile>>();
                container.RegisterType<IRepository<TyreSize>,Repository<TyreSize>>();
                container.RegisterType<IRepository<InspectionMethod>,Repository<InspectionMethod>>();
                container.RegisterType<IRepository<InspectionEquipment>,Repository<InspectionEquipment>>();
                container.RegisterType<IRepository<DetailedException>,Repository<DetailedException>>();
                container.RegisterType<IRepository<RegistrationData>,Repository<RegistrationData>>();
                container.RegisterType<IRepository<User>,Repository<User>>();

                container.RegisterType<ISettingsRepository<WorkshopSettings>,SettingsRepository<WorkshopSettings>>();
                container.RegisterType<ISettingsRepository<PrinterSettings>,SettingsRepository<PrinterSettings>>();
                container.RegisterType<ISettingsRepository<MailSettings>,SettingsRepository<MailSettings>>();
                container.RegisterType<ISettingsRepository<ThemeSettings>,SettingsRepository<ThemeSettings>>();
                container.RegisterType<ISettingsRepository<MiscellaneousSettings>,SettingsRepository<MiscellaneousSettings>>();

                container.RegisterType<IConnectClient,ConnectClient>(new ContainerControlledLifetimeManager());
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError(string.Format("{0}\n\n{1}", Resources.ERR_APPLICATION_CANNOT_CONTINUE, ExceptionPolicy.HandleException(container, ex)));
            }
        }
    }
}