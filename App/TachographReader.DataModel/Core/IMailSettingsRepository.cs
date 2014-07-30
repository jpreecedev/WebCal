using Webcal.DataModel;

namespace Webcal.DataModel
{
    public interface IMailSettingsRepository
    {
        MailSettings GetSettings();

        void Save(MailSettings settings);
    }
}
