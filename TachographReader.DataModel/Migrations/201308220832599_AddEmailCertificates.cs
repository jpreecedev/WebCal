using System.Data.Entity.Migrations;

namespace Webcal.DataModel.Migrations
{
    public partial class AddEmailCertificates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MailSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AutoEmailCertificates = c.Boolean(nullable: false),
                        EmailRecipient = c.String(maxLength: 4000),
                        Subject = c.String(),
                        Body = c.String(),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MailSettings");
        }
    }
}
