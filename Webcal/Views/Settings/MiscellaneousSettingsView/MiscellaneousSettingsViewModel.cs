namespace Webcal.Views.Settings
{
    using System.Collections.Generic;
    using Core;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using Shared;

    public class MiscellaneousSettingsViewModel : BaseSettingsViewModel
    {
        public MiscellaneousSettings Settings { get; set; }

        public ISettingsRepository<MiscellaneousSettings> Repository { get; set; }

        public ICollection<string> DigitalDocumentTypes { get; set; }

        public ICollection<string> AnalogueDocumentTypes { get; set; }
        
        protected override void Load()
        {
            DigitalDocumentTypes = DocumentType.GetDocumentTypes(true);
            AnalogueDocumentTypes = DocumentType.GetDocumentTypes(false);
        }

        protected override void InitialiseRepositories()
        {
            Repository = ContainerBootstrapper.Container.GetInstance<ISettingsRepository<MiscellaneousSettings>>();
            Settings = Repository.GetMiscellaneousSettings();
        }

        public override void Save()
        {
            Repository.Save(Settings);
        }
    }
}