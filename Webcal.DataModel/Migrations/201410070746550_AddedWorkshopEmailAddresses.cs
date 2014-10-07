namespace Webcal.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedWorkshopEmailAddresses : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkshopSettings", "MainEmailAddress", c => c.String(maxLength: 4000));
            AddColumn("dbo.WorkshopSettings", "SecondaryEmailAddress", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WorkshopSettings", "SecondaryEmailAddress");
            DropColumn("dbo.WorkshopSettings", "MainEmailAddress");
        }
    }
}
