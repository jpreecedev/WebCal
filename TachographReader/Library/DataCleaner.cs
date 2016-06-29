namespace TachographReader.Library
{
    using DataModel;

    public static class DataCleaner
    {
        private const string DELETE_QUERY = "UPDATE {0} SET SerializedData = null";

        public static void DeleteStoredPdfDocuments()
        {
            using (var context = new TachographContext())
            {
                context.Database.ExecuteSqlCommand(string.Format(DELETE_QUERY, "GV212Report"));
                context.Database.ExecuteSqlCommand(string.Format(DELETE_QUERY, "LetterForDecommissioningDocuments"));
                context.Database.ExecuteSqlCommand(string.Format(DELETE_QUERY, "QCReport6Month"));
                context.Database.ExecuteSqlCommand(string.Format(DELETE_QUERY, "QCReports"));
                context.Database.ExecuteSqlCommand(string.Format(DELETE_QUERY, "TachographDocuments"));
                context.Database.ExecuteSqlCommand(string.Format(DELETE_QUERY, "UndownloadabilityDocuments"));

                context.SaveChanges();
            }
        }
    }
}