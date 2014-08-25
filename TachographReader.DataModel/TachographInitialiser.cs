namespace Webcal.DataModel
{
    using System.Data.Entity;
    using Migrations;

    public class TachographInitialiser : MigrateDatabaseToLatestVersion<TachographContext, Configuration>
    {
    }
}