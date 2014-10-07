namespace Webcal.Views.Settings
{
    using Core;
    using DataModel;
    using DataModel.Core;

    public class MailSettingsViewModel : BaseSettingsViewModel
    {
        public MailSettings MailSettings { get; set; }

        public IMailSettingsRepository Repository { get; set; }

        public IGeneralSettingsRepository GeneralSettingsRepository { get; set; }

        public WorkshopSettings WorkshopSettings { get; set; }

        protected override void Load()
        {
            MailSettings = Repository.GetSettings();
        }

        protected override void InitialiseRepositories()
        {
            Repository = ContainerBootstrapper.Container.GetInstance<IMailSettingsRepository>();
            GeneralSettingsRepository = ContainerBootstrapper.Container.GetInstance<IGeneralSettingsRepository>();
            WorkshopSettings = GeneralSettingsRepository.GetSettings();
        }

        public override void Save()
        {
            Repository.Save(MailSettings);
            GeneralSettingsRepository.Save(WorkshopSettings);
        }
    }
}