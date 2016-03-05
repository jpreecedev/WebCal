namespace TachographReader.Library.ViewModels
{
    using System;
    using System.Collections.Generic;
    using DataModel;

    public class StatusReportViewModel
    {
        public IList<Technician> Technicians { get; set; }

        public DateTime? TachoCentreLastCheck { get; set; }

        public DateTime? GV212LastCheck { get; set; }
    }
}