namespace TachographReader.DataModel
{
    using System.Data.Entity;
    using Connect.Shared;
    using Connect.Shared.Models;
    using Shared.Models;
    using Shared.Workers;

    public class TachographContext : DbContext
    {
        public TachographContext()
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<CustomDayOfWeek> CustomDayOfWeeks { get; set; }
        public DbSet<TachographDocument> TachographDocuments { get; set; }
        public DbSet<UndownloadabilityDocument> UndownloadabilityDocuments { get; set; }
        public DbSet<LetterForDecommissioningDocument> LetterForDecommissioningDocuments { get; set; }
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
        public DbSet<ThemeSettings> ThemeSettings { get; set; }
        public DbSet<MiscellaneousSettings> MiscellaneousSettings { get; set; }
        public DbSet<WorkerTask> WorkerTasks { get; set; }
        public DbSet<QCReport> QCReports { get; set; }
        public DbSet<QCReport6Month> QCReport6Months { get; set; }
        public DbSet<GV212Report> GV212Reports { get; set; }
    }
}