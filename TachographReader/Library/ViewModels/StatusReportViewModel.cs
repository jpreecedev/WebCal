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

        public ReportItemStatus TachoCentreQuarterlyStatus
        {
            get
            {
                if (TachoCentreLastCheck == null)
                {
                    return ReportItemStatus.Unknown;
                }

                var lastCheck = TachoCentreLastCheck.GetValueOrDefault().Date;
                var now = DateTime.Now.Date;

                if (lastCheck > now)
                {
                    return ReportItemStatus.Unknown;
                }

                var expiration = lastCheck.AddMonths(3).Date;
                var checkDue = expiration.AddDays(-7).Date;

                if (now >= checkDue && now <= expiration)
                {
                    return ReportItemStatus.CheckDue;
                }
                if (now < checkDue)
                {
                    return ReportItemStatus.Ok;
                }
                return ReportItemStatus.Expired;
            }
        }

        public ReportItemStatus GV212Status
        {
            get
            {
                if (GV212LastCheck == null)
                {
                    return ReportItemStatus.Unknown;
                }
                
                var lastCheck = GV212LastCheck.GetValueOrDefault().Date;
                var now = DateTime.Now.Date;

                if (lastCheck > now)
                {
                    return ReportItemStatus.Unknown;
                }
                
                if (lastCheck.Date.Month == now.Month && lastCheck.Date.Year == now.Year)
                {
                    return ReportItemStatus.Ok;
                }

                var lastMonth = now.AddMonths(-1);
                if (lastCheck.Date.Month == lastMonth.Month && lastCheck.Date.Year == lastMonth.Year)
                {
                    return ReportItemStatus.CheckDue;
                }

                return ReportItemStatus.Expired;
            }
        }
    }
}