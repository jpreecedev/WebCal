using System.Data.Entity.Migrations;

namespace Webcal.DataModel.Migrations
{
    public partial class AddSignatureCapture : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "RawImage", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "RawImage");
        }
    }
}
