namespace Webcal.DataModel
{
    using Shared.Core;

    public class MiscellaneousSettings : BaseSettings
    {
        public string DefaultDigitalDocumentType { get; set; }

        public string DefaultAnalogueDocumentType { get; set; }

        public bool ExcludeLogosWhenPrinting { get; set; }

        public int LastMigrationHackId { get; set; }
    }
}