namespace Webcal.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmailSettings_AddPersonaliseCheckBox : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MailSettings", "PersonaliseMyEmails", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MailSettings", "PersonaliseMyEmails");
        }
    }
}
