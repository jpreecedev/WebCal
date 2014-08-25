namespace Webcal.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIndexes : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.CustomDayOfWeeks", new[] { "WorkshopSettings_Id" });
            DropIndex("dbo.VehicleModels", new[] { "VehicleMake_Id" });
            DropIndex("dbo.TachographModels", new[] { "TachographMake_Id" });
            DropIndex("dbo.DriverCardFiles", new[] { "Customer_Id" });

            CreateIndex("dbo.DriverCardFiles", "Customer_Id");
            CreateIndex("dbo.TachographModels", "TachographMake_Id");
            CreateIndex("dbo.VehicleModels", "VehicleMake_Id");
            CreateIndex("dbo.CustomDayOfWeeks", "WorkshopSettings_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.CustomDayOfWeeks", new[] { "WorkshopSettings_Id" });
            DropIndex("dbo.VehicleModels", new[] { "VehicleMake_Id" });
            DropIndex("dbo.TachographModels", new[] { "TachographMake_Id" });
            DropIndex("dbo.DriverCardFiles", new[] { "Customer_Id" });
        }
    }
}
