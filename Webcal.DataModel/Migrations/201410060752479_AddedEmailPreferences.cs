namespace Webcal.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEmailPreferences : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkshopSettings", "SendToCustomer", c => c.Boolean(nullable: false));
            AddColumn("dbo.WorkshopSettings", "SendToOffice", c => c.Boolean(nullable: false));
            AddColumn("dbo.WorkshopSettings", "DoNotSend", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WorkshopSettings", "DoNotSend");
            DropColumn("dbo.WorkshopSettings", "SendToOffice");
            DropColumn("dbo.WorkshopSettings", "SendToCustomer");
        }
    }
}
