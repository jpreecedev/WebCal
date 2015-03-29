namespace TachographReader.Shared.Workers
{
    using System.Configuration;

    public class PluginsElementCollection : BaseElementCollection<Plugin>
    {
        public PluginsElementCollection()
        {
            var element = (Plugin) CreateNewElement();
            Add(element, element.Name);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Plugin) element).Name;
        }
    }
}