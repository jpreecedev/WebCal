namespace TachographReader.Core
{
    using System.Linq;

    public class StartupArguments
    {
        [CommandLineArgument("Culture")]
        public string Culture { get; set; }

        public void Parse(string[] args)
        {
            if (args == null)
            {
                return;
            }

            foreach (var argument in args)
            {
                var split = argument.Split(':');
                if (split.Length == 2)
                {
                    SetAttribute(split[0].Remove(0, 1), split[1]);
                }
            }
        }

        private void SetAttribute(string name, string value)
        {
            var properties = GetType().GetProperties();

            foreach (var propertyInfo in properties)
            {
                var attribute = propertyInfo.GetCustomAttributes(typeof (CommandLineArgumentAttribute), false).FirstOrDefault();
                if (attribute != null)
                {
                    var theAttribute = (CommandLineArgumentAttribute) attribute;
                    if (!string.IsNullOrEmpty(theAttribute.Value) && string.Equals(theAttribute.Value, name))
                    {
                        propertyInfo.SetValue(this, value);
                    }
                }
            }
        }
    }
}