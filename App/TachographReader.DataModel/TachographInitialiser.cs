using System.Data.Entity;
using Webcal.DataModel.Migrations;

namespace Webcal.DataModel
{
    public class TachographInitialiser : MigrateDatabaseToLatestVersion<TachographContext, Configuration>
    {

    }
}
