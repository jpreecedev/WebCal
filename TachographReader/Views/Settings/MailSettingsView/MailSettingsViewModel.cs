namespace TachographReader.Views.Settings
{
    using Core;
    using DataModel;
    using Shared;

    public class MailSettingsViewModel : BaseSettingsViewModel
    {
        public MailSettings MailSettings { get; set; }
        public ISettingsRepository<MailSettings> Repository { get; set; }
        
        protected override void Load()
        {
            MailSettings = Repository.Get();
        }

        protected override void InitialiseRepositories()
        {
            Repository = GetInstance<ISettingsRepository<MailSettings>>();
        }

        public override void Save()
        {
            Repository.Save(MailSettings);
        }
    }
}