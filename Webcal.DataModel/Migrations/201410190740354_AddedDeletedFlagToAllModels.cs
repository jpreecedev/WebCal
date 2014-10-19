namespace Webcal.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDeletedFlagToAllModels : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerContacts", "Deleted", c => c.DateTime());
            AddColumn("dbo.DriverCardFiles", "Deleted", c => c.DateTime());
            AddColumn("dbo.DetailedExceptions", "Deleted", c => c.DateTime());
            AddColumn("dbo.InspectionEquipments", "Deleted", c => c.DateTime());
            AddColumn("dbo.InspectionMethods", "Deleted", c => c.DateTime());
            AddColumn("dbo.MailSettings", "Deleted", c => c.DateTime());
            AddColumn("dbo.PrinterSettings", "Deleted", c => c.DateTime());
            AddColumn("dbo.RegistrationData", "Deleted", c => c.DateTime());
            AddColumn("dbo.TachographDocuments", "Deleted", c => c.DateTime());
            AddColumn("dbo.TachographFiles", "Deleted", c => c.DateTime());
            AddColumn("dbo.TachographMakes", "Deleted", c => c.DateTime());
            AddColumn("dbo.TachographModels", "Deleted", c => c.DateTime());
            AddColumn("dbo.Technicians", "Deleted", c => c.DateTime());
            AddColumn("dbo.TyreSizes", "Deleted", c => c.DateTime());
            AddColumn("dbo.UndownloadabilityDocuments", "Deleted", c => c.DateTime());
            AddColumn("dbo.Users", "Deleted", c => c.DateTime());
            AddColumn("dbo.VehicleMakes", "Deleted", c => c.DateTime());
            AddColumn("dbo.VehicleModels", "Deleted", c => c.DateTime());
            AddColumn("dbo.WorkshopCardFiles", "Deleted", c => c.DateTime());
            AddColumn("dbo.WorkshopSettings", "Deleted", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.WorkshopSettings", "Deleted");
            DropColumn("dbo.WorkshopCardFiles", "Deleted");
            DropColumn("dbo.VehicleModels", "Deleted");
            DropColumn("dbo.VehicleMakes", "Deleted");
            DropColumn("dbo.Users", "Deleted");
            DropColumn("dbo.UndownloadabilityDocuments", "Deleted");
            DropColumn("dbo.TyreSizes", "Deleted");
            DropColumn("dbo.Technicians", "Deleted");
            DropColumn("dbo.TachographModels", "Deleted");
            DropColumn("dbo.TachographMakes", "Deleted");
            DropColumn("dbo.TachographFiles", "Deleted");
            DropColumn("dbo.TachographDocuments", "Deleted");
            DropColumn("dbo.RegistrationData", "Deleted");
            DropColumn("dbo.PrinterSettings", "Deleted");
            DropColumn("dbo.MailSettings", "Deleted");
            DropColumn("dbo.InspectionMethods", "Deleted");
            DropColumn("dbo.InspectionEquipments", "Deleted");
            DropColumn("dbo.DetailedExceptions", "Deleted");
            DropColumn("dbo.DriverCardFiles", "Deleted");
            DropColumn("dbo.CustomerContacts", "Deleted");
        }
    }
}
