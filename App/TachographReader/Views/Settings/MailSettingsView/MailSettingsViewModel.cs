using StructureMap;
using Webcal.Core;
using Webcal.DataModel;

namespace Webcal.Views.Settings
{
    public class MailSettingsViewModel : BaseSettingsViewModel
    {
        #region Public Properties

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

        #endregion

        #region Overrides

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

        #endregion
    }
}