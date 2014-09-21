namespace Webcal.Shared.Workers
{
    using System.Configuration;

    public class WebcalConfigurationSection : ConfigurationSection
    {
        private static readonly WebcalConfigurationSection _instance = (WebcalConfigurationSection) ConfigurationManager.GetSection("Webcal");

        [ConfigurationProperty("Plugins")]
        [ConfigurationCollection(typeof (PluginsElementCollection), AddItemName = "Plugin")]
        public PluginsElementCollection PluginsCollection
        {
            get { return (PluginsElementCollection) base["Plugins"]; }
        }

        public static WebcalConfigurationSection Instance
        {
            get { return _instance; }
        }
    }
}