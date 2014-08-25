using System.Data.Entity.Migrations;

namespace Webcal.DataModel.Migrations
{
    public partial class RemoveEmailFromMailSettings : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.MailSettings", "EmailRecipient");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MailSettings", "EmailRecipient", c => c.String(maxLength: 4000));
        }
    }
}
