namespace TachographReader.Shared.Workers
{
    using System.Configuration;

    public class Plugin : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string) this["name"]; }
        }

        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get { return (string) this["path"]; }
        }
    }
}