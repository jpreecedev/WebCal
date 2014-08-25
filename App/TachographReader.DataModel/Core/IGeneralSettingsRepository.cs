namespace Webcal.DataModel.Core
{
    public interface IGeneralSettingsRepository
    {
        WorkshopSettings GetSettings();

        void Save(WorkshopSettings settings);

        CustomerContact GetCustomerSettings(string name);
    }
}