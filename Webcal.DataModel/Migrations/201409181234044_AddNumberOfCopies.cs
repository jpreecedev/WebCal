namespace Webcal.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNumberOfCopies : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PrinterSettings", "DefaultNumberOfCopies", c => c.Int(nullable: false));
            AddColumn("dbo.PrinterSettings", "LabelNumberOfCopies", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PrinterSettings", "LabelNumberOfCopies");
            DropColumn("dbo.PrinterSettings", "DefaultNumberOfCopies");
        }
    }
}
