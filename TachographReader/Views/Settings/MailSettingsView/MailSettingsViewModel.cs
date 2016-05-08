namespace TachographReader.Views.Settings
{
    using Connect.Shared;
    using Core;
    using DataModel;
    using DataModel.Library;
    using Shared;

    public class MailSettingsViewModel : BaseSettingsViewModel
    {
        public MailSettings MailSettings { get; set; }
        public ISettingsRepository<MailSettings> Repository { get; set; }
        public ISettingsRepository<WorkshopSettings> SettingsRepository { get; set; }
        public WorkshopSettings WorkshopSettings { get; set; }

        protected override void Load()
        {
            MailSettings = Repository.Get();
        }

        protected override void InitialiseRepositories()
        {
            Repository = GetInstance<ISettingsRepository<MailSettings>>();
            SettingsRepository = GetInstance<ISettingsRepository<WorkshopSettings>>();
            WorkshopSettings = SettingsRepository.GetWorkshopSettings();
        }

        public override void Save()
        {
            Repository.Save(MailSettings);
            SettingsRepository.Save(WorkshopSettings);
        }
    }
}