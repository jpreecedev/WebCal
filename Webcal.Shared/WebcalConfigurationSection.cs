namespace Webcal.Shared
{
    using System.Configuration;
    using Connect;
    using Workers;

    public class WebcalConfigurationSection : ConfigurationSection
    {
        private static readonly WebcalConfigurationSection _instance = (WebcalConfigurationSection) ConfigurationManager.GetSection("Webcal");

        [ConfigurationProperty("Plugins")]
        [ConfigurationCollection(typeof (PluginsElementCollection), AddItemName = "Plugin")]
        public PluginsElementCollection PluginsCollection
        {
            get { return (PluginsElementCollection) base["Plugins"]; }
        }

        [ConfigurationProperty("Connect")]
        [ConfigurationCollection(typeof(PluginsElementCollection), AddItemName = "Connect")]
        public ConnectElementCollection ConnectCollection
        {
            get { return (ConnectElementCollection)base["Connect"]; }
        }

        public static WebcalConfigurationSection Instance
        {
            get { return _instance; }
        }
    }
}