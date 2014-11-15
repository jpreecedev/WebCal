namespace Webcal.DataModel.Core
{
    using System;
    using System.Windows;
    using Properties;
    using Repositories;
    using Shared;
    using StructureMap;
    using StructureMap.Configuration.DSL;
    using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

    public static class ContainerBootstrapper
    {
        private static readonly Container _container;

        static ContainerBootstrapper()
        {
            _container = new Container(expression => { Configure(expression); });
        }

        public static Container Container
        {
            get { return _container; }
        }

        public static bool Configure(ConfigurationExpression x)
        {
            try
            {
                x.AddRegistry<ContainerRegistry>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}\n\n{1}", Resources.ERR_APPLICATION_CANNOT_CONTINUE, ExceptionPolicy.HandleException(Container, ex)), Resources.TXT_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private class ContainerRegistry : Registry
        {
            public ContainerRegistry()
            {
                For<IRepository<VehicleMake>>().Use<VehicleRepository>();
                For<IRepository<TachographDocument>>().Use<TachographDocumentRepository>();
                For<IRepository<TachographMake>>().Use<TachographMakesRepository>();
                For<IRepository<Technician>>().Use<TechnicianRepository>();

                For<IRepository<UndownloadabilityDocument>>().Use<Repository<UndownloadabilityDocument>>();
                For<IRepository<CustomerContact>>().Use<Repository<CustomerContact>>();
                For<IRepository<TachographFile>>().Use<Repository<TachographFile>>();
                For<IRepository<DriverCardFile>>().Use<Repository<DriverCardFile>>();
                For<IRepository<WorkshopCardFile>>().Use<Repository<WorkshopCardFile>>();
                For<IRepository<TyreSize>>().Use<Repository<TyreSize>>();
                For<IRepository<InspectionMethod>>().Use<Repository<InspectionMethod>>();
                For<IRepository<InspectionEquipment>>().Use<Repository<InspectionEquipment>>();
                For<IRepository<DetailedException>>().Use<Repository<DetailedException>>();
                For<IRepository<RegistrationData>>().Use<Repository<RegistrationData>>();
                For<IRepository<User>>().Use<Repository<User>>();

                For<ISettingsRepository<WorkshopSettings>>().Use<SettingsRepository<WorkshopSettings>>();
                For<ISettingsRepository<PrinterSettings>>().Use<SettingsRepository<PrinterSettings>>();
                For<ISettingsRepository<MailSettings>>().Use<SettingsRepository<MailSettings>>();
                For<ISettingsRepository<ThemeSettings>>().Use<SettingsRepository<ThemeSettings>>();
            }
        }
    }
}