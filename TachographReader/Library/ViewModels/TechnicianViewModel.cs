namespace TachographReader.Library.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataModel;

    public class TechnicianViewModel
    {
        public TechnicianViewModel(ICollection<ReportDocumentViewModel> documents, List<DateTime> last12Months, Technician technician)
        {
            Technician = technician;
            JobsDoneInLast12Months = documents.Count(c => c.Technician == technician.Name);
            JobsMonthByMonth = new Dictionary<DateTime, int>();

            foreach (var month in last12Months)
            {
                JobsMonthByMonth.Add(month, documents.Count(c => c.Technician == technician.Name && c.Created.Date >= month.Date && c.Created.Date <= month.AddMonths(1).AddDays(-1)));
            }
        }

        public Technician Technician { get; }

        public int JobsDoneInLast12Months { get; }

        public Dictionary<DateTime, int> JobsMonthByMonth { get;  }
    }
}