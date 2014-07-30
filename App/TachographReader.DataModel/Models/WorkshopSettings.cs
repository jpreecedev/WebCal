using System.Collections.Generic;

namespace Webcal.DataModel
{
    public class WorkshopSettings : BaseModel
    {
        #region Public Properties

        public int Id { get; set; }

        public bool AutoBackup { get; set; }

        public string BackupFilePath { get; set; }

        public IList<CustomDayOfWeek> BackupDaysOfWeek { get; set; }

        public string Office { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Town { get; set; }

        public string PostCode { get; set; }

        public string WorkshopName { get; set; }

        public string PhoneNumber { get; set; }

        public bool AutoPrintLabels { get; set; }

        #endregion
    }
}
