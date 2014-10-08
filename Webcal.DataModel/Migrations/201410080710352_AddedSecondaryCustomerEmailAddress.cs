namespace Webcal.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSecondaryCustomerEmailAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerContacts", "SecondaryEmail", c => c.String(maxLength: 4000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerContacts", "SecondaryEmail");
        }
    }
}
