namespace TachographReader.Library.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using Shared;

    public class StatusReportViewModel
    {
        public StatusReportViewModel()
        {
            var generalSettingsRepository = ContainerBootstrapper.Resolve<ISettingsRepository<WorkshopSettings>>().GetWorkshopSettings();
            TachoCentreLastCheck = generalSettingsRepository.CentreQuarterlyCheckDate;
            GV212LastCheck = generalSettingsRepository.MonthlyGV212Date;
        }

        public StatusReportViewModel(IList<Technician> technicians) : this()
        {
            Technicians = technicians;
        }

        public IList<Technician> Technicians { get; }

        public DateTime? TachoCentreLastCheck { get; set; }

        public DateTime? GV212LastCheck { get; set; }

        public bool IsUpToDate()
        {
            var techniciansUpToDate = true;
            if (Technicians != null)
            {
                foreach (var technician in Technicians)
                {
                    if (technician.HalfYearStatus() != ReportItemStatus.Ok)
                    {
                        techniciansUpToDate = false;
                        break;
                    }
                    if (technician.ThreeYearStatus() != ReportItemStatus.Ok)
                    {
                        techniciansUpToDate = false;
                        break;
                    }
                }   
            }

            if ((Technicians == null || Technicians.Count == 0) || (Technicians.All(c => c.DateOfLast3YearCheck == null && c.DateOfLastCheck == null)))
            {
                return TachoCentreQuarterlyStatus == ReportItemStatus.Unknown && GV212Status == ReportItemStatus.Unknown;
            }

            return techniciansUpToDate && TachoCentreQuarterlyStatus == ReportItemStatus.Ok && GV212Status == ReportItemStatus.Ok;
        }

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

        public bool IsGV212Due()
        {
            return GV212Status == ReportItemStatus.CheckDue || GV212Status == ReportItemStatus.Expired;
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

                return ReportItemStatus.Expired;
            }
        }
    }
}