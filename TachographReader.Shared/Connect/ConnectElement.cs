namespace TachographReader.Shared.Connect
{
    using System.Configuration;

    public class ConnectElement : ConfigurationElement
    {
        [ConfigurationProperty("address", IsRequired = true, IsKey = true)]
        public string Address
        {
            get { return (string) this["address"]; }
        }
    }
}