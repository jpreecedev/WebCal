namespace Webcal.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserManagement : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(maxLength: 4000),
                        Password = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.WorkshopSettings", "PhoneNumber", c => c.String(maxLength: 4000));
            DropColumn("dbo.WorkshopSettings", "RawImage");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WorkshopSettings", "RawImage", c => c.Binary());
            DropColumn("dbo.WorkshopSettings", "PhoneNumber");
            DropTable("dbo.Users");
        }
    }
}
