namespace TachographReader.Shared
{
    using System.Configuration;
    using Workers;

    public class WebcalConfigurationSection : ConfigurationSection
    {
        private static readonly WebcalConfigurationSection _instance = (WebcalConfigurationSection)ConfigurationManager.GetSection("TachographReader");

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