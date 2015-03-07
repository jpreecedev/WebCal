using System.Data.Entity.Migrations;

namespace Webcal.DataModel.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<TachographContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }
    }
}
