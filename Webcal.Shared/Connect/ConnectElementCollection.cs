namespace Webcal.Shared.Connect
{
    using System.Configuration;
    using Workers;

    public class ConnectElementCollection : BaseElementCollection<ConnectElement>
    {
        public ConnectElementCollection()
        {
            var element = (ConnectElement) CreateNewElement();
            Add(element, element.Address);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ConnectElement) element).Address;
        }
    }
}