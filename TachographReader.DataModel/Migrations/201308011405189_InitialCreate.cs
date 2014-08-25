using System.Data.Entity.Migrations;

namespace Webcal.DataModel.Migrations
{
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TachographDocuments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VIN = c.String(maxLength: 4000),
                        VehicleMake = c.String(maxLength: 4000),
                        VehicleModel = c.String(maxLength: 4000),
                        TyreSize = c.String(maxLength: 4000),
                        VehicleType = c.String(maxLength: 4000),
                        WFactor = c.String(maxLength: 4000),
                        KFactor = c.String(maxLength: 4000),
                        LFactor = c.String(maxLength: 4000),
                        OdometerReading = c.String(maxLength: 4000),
                        Tampered = c.Boolean(nullable: false),
                        InvoiceNumber = c.String(maxLength: 4000),
                        InspectionInfo = c.String(maxLength: 4000),
                        TachographHasAdapter = c.Boolean(nullable: false),
                        TachographAdapterSerialNumber = c.String(maxLength: 4000),
                        TachographAdapterLocation = c.String(maxLength: 4000),
                        TachographCableColor = c.String(maxLength: 4000),
                        MinorWorkDetails = c.String(maxLength: 4000),
                        TachographType = c.String(maxLength: 4000),
                        CardSerialNumber = c.String(maxLength: 4000),
                        CalibrationTime = c.DateTime(),
                        IsDigital = c.Boolean(nullable: false),
                        Created = c.DateTime(nullable: false),
                        DocumentType = c.String(maxLength: 4000),
                        Office = c.String(maxLength: 4000),
                        RegistrationNumber = c.String(maxLength: 4000),
                        TachographMake = c.String(maxLength: 4000),
                        TachographModel = c.String(maxLength: 4000),
                        SerialNumber = c.String(maxLength: 4000),
                        InspectionDate = c.DateTime(),
                        Technician = c.String(maxLength: 4000),
                        CustomerContact = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UndownloadabilityDocuments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Created = c.DateTime(nullable: false),
                        DocumentType = c.String(maxLength: 4000),
                        Office = c.String(maxLength: 4000),
                        RegistrationNumber = c.String(maxLength: 4000),
                        TachographMake = c.String(maxLength: 4000),
                        TachographModel = c.String(maxLength: 4000),
                        SerialNumber = c.String(maxLength: 4000),
                        InspectionDate = c.DateTime(),
                        Technician = c.String(maxLength: 4000),
                        CustomerContact = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VehicleMakes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VehicleModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                        VehicleMake_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VehicleMakes", t => t.VehicleMake_Id)
                .Index(t => t.VehicleMake_Id);
            
            CreateTable(
                "dbo.TachographMakes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TachographModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                        TachographMake_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TachographMakes", t => t.TachographMake_Id)
                .Index(t => t.TachographMake_Id);
            
            CreateTable(
                "dbo.TachographFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Customer = c.String(maxLength: 4000),
                        RegistrationNumber = c.String(maxLength: 4000),
                        FileName = c.String(maxLength: 4000),
                        SerializedFile = c.Binary(),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DriverCardFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Driver = c.String(maxLength: 4000),
                        FileName = c.String(maxLength: 4000),
                        SerializedFile = c.Binary(),
                        Date = c.DateTime(nullable: false),
                        Customer_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CustomerContacts", t => t.Customer_Id)
                .Index(t => t.Customer_Id);
            
            CreateTable(
                "dbo.CustomerContacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                        Email = c.String(maxLength: 4000),
                        Address = c.String(maxLength: 4000),
                        PostCode = c.String(maxLength: 4000),
                        Town = c.String(maxLength: 4000),
                        PhoneNumber = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WorkshopCardFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Workshop = c.String(maxLength: 4000),
                        FileName = c.String(maxLength: 4000),
                        SerializedFile = c.Binary(),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WorkshopSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AutoBackup = c.Boolean(nullable: false),
                        BackupFilePath = c.String(maxLength: 4000),
                        Office = c.String(maxLength: 4000),
                        Address1 = c.String(maxLength: 4000),
                        Address2 = c.String(maxLength: 4000),
                        Town = c.String(maxLength: 4000),
                        PostCode = c.String(maxLength: 4000),
                        WorkshopName = c.String(maxLength: 4000),
                        RawImage = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CustomDayOfWeeks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DayOfWeek = c.String(maxLength: 4000),
                        WorkshopSettings_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkshopSettings", t => t.WorkshopSettings_Id)
                .Index(t => t.WorkshopSettings_Id);
            
            CreateTable(
                "dbo.TyreSizes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Size = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Technicians",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.InspectionMethods",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.InspectionEquipments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DetailedExceptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ExceptionDetails = c.String(),
                        ApplicationName = c.String(maxLength: 4000),
                        RawImage = c.Binary(),
                        Occurred = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RegistrationData",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LicenseKey = c.String(maxLength: 4000),
                        CompanyName = c.String(maxLength: 4000),
                        SealNumber = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PrinterSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AlwaysAskForPrinter = c.Boolean(nullable: false),
                        UseDefaultPrinter = c.Boolean(nullable: false),
                        DefaultPrinterName = c.String(maxLength: 4000),
                        DefaultLabelPrinter = c.String(maxLength: 4000),
                        IsPortrait = c.Boolean(nullable: false),
                        IsLandscape = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.CustomDayOfWeeks", new[] { "WorkshopSettings_Id" });
            DropIndex("dbo.DriverCardFiles", new[] { "Customer_Id" });
            DropIndex("dbo.TachographModels", new[] { "TachographMake_Id" });
            DropIndex("dbo.VehicleModels", new[] { "VehicleMake_Id" });
            DropForeignKey("dbo.CustomDayOfWeeks", "WorkshopSettings_Id", "dbo.WorkshopSettings");
            DropForeignKey("dbo.DriverCardFiles", "Customer_Id", "dbo.CustomerContacts");
            DropForeignKey("dbo.TachographModels", "TachographMake_Id", "dbo.TachographMakes");
            DropForeignKey("dbo.VehicleModels", "VehicleMake_Id", "dbo.VehicleMakes");
            DropTable("dbo.PrinterSettings");
            DropTable("dbo.RegistrationData");
            DropTable("dbo.DetailedExceptions");
            DropTable("dbo.InspectionEquipments");
            DropTable("dbo.InspectionMethods");
            DropTable("dbo.Technicians");
            DropTable("dbo.TyreSizes");
            DropTable("dbo.CustomDayOfWeeks");
            DropTable("dbo.WorkshopSettings");
            DropTable("dbo.WorkshopCardFiles");
            DropTable("dbo.CustomerContacts");
            DropTable("dbo.DriverCardFiles");
            DropTable("dbo.TachographFiles");
            DropTable("dbo.TachographModels");
            DropTable("dbo.TachographMakes");
            DropTable("dbo.VehicleModels");
            DropTable("dbo.VehicleMakes");
            DropTable("dbo.UndownloadabilityDocuments");
            DropTable("dbo.TachographDocuments");
        }
    }
}
