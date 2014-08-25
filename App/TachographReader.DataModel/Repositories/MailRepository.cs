namespace Webcal.DataModel.Repositories
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    public class MailRepository : BaseRepository, IMailSettingsRepository
    {
        public MailSettings GetSettings()
        {
            List<MailSettings> mailSettings = Safely(() => Context.MailSettings.ToList());

            if (mailSettings.Count == 0)
                return new MailSettings();
            if (mailSettings.Count == 1)
                return mailSettings.First();

            return GetMailSettings(mailSettings);
        }

        public void Save(MailSettings settings)
        {
            Safely(() =>
            {
                MailSettings existing = Context.MailSettings.Find(settings.Id);
                if (existing != null)
                    Context.Entry(settings).State = EntityState.Modified;
                else
                    Context.Set<MailSettings>().Add(settings);

                Context.SaveChanges();
            });
        }


        private static MailSettings GetMailSettings(IEnumerable<MailSettings> all)
        {
            return all.First(); //Could be a problem here
        }
    }
}