namespace TachographReader.DataModel
{
    using Connect.Shared;

    public class MiscellaneousSettings : BaseSettings
    {
        public string DefaultDigitalDocumentType { get; set; }

        public string DefaultAnalogueDocumentType { get; set; }

        public bool ExcludeLogosWhenPrinting { get; set; }

        public int LastMigrationHackId { get; set; }
    }
}