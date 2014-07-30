using System.Data.Entity;
using Webcal.Shared;
using Webcal.DataModel;

namespace Webcal.DataModel
{
    public class TachographContext : DbContext
    {
        public TachographContext(): base()
        {
            Configuration.LazyLoadingEnabled = false;

            var type1 = typeof(System.Data.Entity.SqlServer.SqlProviderServices);
            var type2 = typeof(System.Data.Entity.SqlServerCompact.SqlCeProviderServices);

        }

        public DbSet<TachographDocument> TachographDocuments { get; set; }

        public DbSet<UndownloadabilityDocument> UndownloadabilityDocuments { get; set; }

        public DbSet<VehicleMake> VehicleMakes { get; set; }

        public DbSet<TachographMake> TachographMakes { get; set; }

        public DbSet<TachographFile> TachographFiles { get; set; }

        public DbSet<DriverCardFile> DriverCardFiles { get; set; }

        public DbSet<WorkshopCardFile> WorkshopCardFiles { get; set; }

        public DbSet<CustomerContact> CustomerContacts { get; set; }

        public DbSet<WorkshopSettings> WorkshopSettings { get; set; }

        public DbSet<TyreSize> TyreSizes { get; set; }

        public DbSet<Technician> Technicians { get; set; }

        public DbSet<InspectionMethod> InspectionMethods { get; set; }

        public DbSet<InspectionEquipment> InspectionEquipments { get; set; }

        public DbSet<DetailedException> Exceptions { get; set; }

        public DbSet<RegistrationData> RegistrationData { get; set; }

        public DbSet<PrinterSettings> PrinterSettings { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<MailSettings> MailSettings { get; set; }
    }
}