namespace Webcal.Views.Settings
{
    using Core;
    using DataModel;
    using StructureMap;

    public class MailSettingsViewModel : BaseSettingsViewModel
    {
        public MailSettings MailSettings { get; set; }

        public IMailSettingsRepository Repository { get; set; }

        public bool DontSendEmails
        {
            get
            {
                return MailSettings != null &&
                       !MailSettings.AllowEditingOfEmail &&
                       !MailSettings.AutoEmailCertificates &&
                       !MailSettings.PersonaliseMyEmails;
            }
        }

        protected override void Load()
        {
            MailSettings = Repository.GetSettings();
        }

        protected override void InitialiseRepositories()
        {
            Repository = ObjectFactory.GetInstance<IMailSettingsRepository>();
        }

        public override void Save()
        {
            Repository.Save(MailSettings);
        }
    }
}