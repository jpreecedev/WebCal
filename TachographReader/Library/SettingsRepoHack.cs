namespace TachographReader.Library
{
    using Connect.Shared;
    using DataModel.Core;
    using DataModel.Library;
    using Shared;

    public class SettingsRepoHack
    {
        private static SettingsRepoHack _instance;
        private WorkshopSettings _workshopSettings;

        public WorkshopSettings Settings
        {
            get { return _workshopSettings ?? (_workshopSettings = GetWorkshopSettings()); }
        }

        public static SettingsRepoHack GetInstance()
        {
            return _instance ?? (_instance = new SettingsRepoHack());
        }

        public static void KillInstance()
        {
            _instance = null;
        }

        public void SaveSettings()
        {
            var repository = ContainerBootstrapper.Resolve<ISettingsRepository<WorkshopSettings>>();
            repository.Save(_workshopSettings);
        }

        private static WorkshopSettings GetWorkshopSettings()
        {
            var repository = ContainerBootstrapper.Resolve<ISettingsRepository<WorkshopSettings>>();
            return repository.GetWorkshopSettings();
        }
    }
}