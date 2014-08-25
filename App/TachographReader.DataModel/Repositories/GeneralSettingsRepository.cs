namespace Webcal.DataModel.Repositories
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using Core;

    public class GeneralSettingsRepository : BaseRepository, IGeneralSettingsRepository
    {
        public WorkshopSettings GetSettings()
        {
            List<WorkshopSettings> workshopSettings = Safely(() => Context.WorkshopSettings.Include("BackupDaysOfWeek")).ToList();

            if (workshopSettings.Count == 0)
                return null;
            if (workshopSettings.Count == 1)
                return workshopSettings.First();

            return GetWorkshopSettings(workshopSettings);
        }

        public CustomerContact GetCustomerSettings(string name)
        {
            List<CustomerContact> customerSettings = Safely(() => Context.CustomerContacts).ToList();

            return GetCustomerContactSettings(customerSettings, name);
        }

        public void Save(WorkshopSettings settings)
        {
            Safely(() =>
            {
                Context.WorkshopSettings.Attach(settings);
                Context.Entry(settings).State = EntityState.Modified;
                Context.SaveChanges();
            });
        }


        private static WorkshopSettings GetWorkshopSettings(IEnumerable<WorkshopSettings> all)
        {
            return all.FirstOrDefault(w => !string.IsNullOrEmpty(w.Address1) ||
                                           !string.IsNullOrEmpty(w.Address2) ||
                                           !string.IsNullOrEmpty(w.Office) ||
                                           !string.IsNullOrEmpty(w.PostCode) ||
                                           !string.IsNullOrEmpty(w.Town) ||
                                           !string.IsNullOrEmpty(w.WorkshopName));
        }

        private static CustomerContact GetCustomerContactSettings(IEnumerable<CustomerContact> all, string name)
        {
            return all.FirstOrDefault(c => c.Name == name);
        }
    }
}