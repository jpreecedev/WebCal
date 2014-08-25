namespace Webcal.DataModel.Core
{
    using System;
    using System.Data.Entity;
    using System.IO;
    using System.Windows;
    using Properties;
    using Repositories;
    using Shared;
    using StructureMap;
    using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

    public static class ContainerBootstrapper
    {
        public static bool Initialise(IInitializationExpression x)
        {
            try
            {
                if (File.Exists(Environment.SpecialFolder.MyDocuments + "\\webcal.sdf"))
                    Database.SetInitializer(new TachographInitialiser());

                x.For<IRepository<VehicleMake>>().Use<VehicleRepository>();
                x.For<IRepository<UndownloadabilityDocument>>().Use<UndownloadabilityDocumentRepository>();
                x.For<IRepository<TachographDocument>>().Use<TachographDocumentRepository>();
                x.For<IRepository<CustomerContact>>().Use<CustomerContactRepository>();
                x.For<IRepository<TachographFile>>().Use<TachographFileRepository>();
                x.For<IRepository<DriverCardFile>>().Use<DriverCardFileRepository>();
                x.For<IRepository<WorkshopCardFile>>().Use<WorkshopCardFileRepository>();
                x.For<IGeneralSettingsRepository>().Use<GeneralSettingsRepository>();
                x.For<IRepository<TyreSize>>().Use<TyreSizeRepository>();
                x.For<IRepository<TachographMake>>().Use<TachographMakesRepository>();
                x.For<IRepository<Technician>>().Use<TechnicianRepository>();
                x.For<IRepository<InspectionMethod>>().Use<InspectionMethodsRepository>();
                x.For<IRepository<InspectionEquipment>>().Use<InspectionEquipmentRepository>();
                x.For<IRepository<DetailedException>>().Use<ExceptionRepository>();
                x.For<IRepository<RegistrationData>>().Use<RegistrationRepository>();
                x.For<IPrinterSettingsRepository>().Use<PrinterSettingsRepository>();
                x.For<IRepository<User>>().Use<UserRepository>();
                x.For<IMailSettingsRepository>().Use<MailRepository>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}\n\n{1}", Resources.ERR_APPLICATION_CANNOT_CONTINUE, ExceptionPolicy.HandleException(ex)), Resources.TXT_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public static bool Configure(ConfigurationExpression x, string dbLocation)
        {
            try
            {
                if (!File.Exists(dbLocation))
                    Database.SetInitializer(new TachographInitialiser());
                x.For<IRepository<VehicleMake>>().Use(() => new VehicleRepository());
                x.For<IRepository<UndownloadabilityDocument>>().Use(() => new UndownloadabilityDocumentRepository());
                x.For<IRepository<TachographDocument>>().Use(() => new TachographDocumentRepository());
                x.For<IRepository<CustomerContact>>().Use(() => new CustomerContactRepository());
                x.For<IRepository<TachographFile>>().Use(() => new TachographFileRepository());
                x.For<IRepository<DriverCardFile>>().Use(() => new DriverCardFileRepository());
                x.For<IRepository<WorkshopCardFile>>().Use(() => new WorkshopCardFileRepository());
                x.For<IGeneralSettingsRepository>().Use(() => new GeneralSettingsRepository());
                x.For<IRepository<TyreSize>>().Use(() => new TyreSizeRepository());
                x.For<IRepository<TachographMake>>().Use(() => new TachographMakesRepository());
                x.For<IRepository<Technician>>().Use(() => new TechnicianRepository());
                x.For<IRepository<InspectionMethod>>().Use(() => new InspectionMethodsRepository());
                x.For<IRepository<InspectionEquipment>>().Use(() => new InspectionEquipmentRepository());
                x.For<IRepository<DetailedException>>().Use(() => new ExceptionRepository());
                x.For<IRepository<RegistrationData>>().Use(() => new RegistrationRepository());
                x.For<IPrinterSettingsRepository>().Use(() => new PrinterSettingsRepository());
                x.For<IRepository<User>>().Use(() => new UserRepository());
                x.For<IMailSettingsRepository>().Use(() => new MailRepository());
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}\n\n{1}", Resources.ERR_APPLICATION_CANNOT_CONTINUE, ExceptionPolicy.HandleException(ex)), Resources.TXT_ERROR, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
    }
}