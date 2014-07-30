using System.Data.Entity.Migrations;

namespace Webcal.DataModel.Migrations
{
    public partial class AddAutoPrintLabels : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkshopSettings", "AutoPrintLabels", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WorkshopSettings", "AutoPrintLabels");
        }
    }
}
