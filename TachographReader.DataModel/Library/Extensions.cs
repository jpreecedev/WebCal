namespace Webcal.DataModel.Library
{
    using System;
    using System.Xml.Linq;

    public static class Extensions
    {
        public static bool SafelyGetValueAsBool(this XAttribute attribute)
        {
            try
            {
                return (bool) attribute;
            }
            catch
            {
                return false;
            }
        }

        public static string SafelyGetValue(this XAttribute attribute)
        {
            try
            {
                return attribute.Value;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string SafelyGetValue(this XElement element)
        {
            try
            {
                return element.Value;
            }
            catch
            {
                return string.Empty;
            }
        }


        public static DateTime SafelyGetValueAsDateTime(this XElement element)
        {
            try
            {
                DateTime parsed;
                if (DateTime.TryParse(element.Value, out parsed))
                    return parsed;

                return default(DateTime);
            }
            catch
            {
                return default(DateTime);
            }
        }

        public static double SafelyGetValueAsDouble(this XElement element)
        {
            try
            {
                double parsed;
                if (double.TryParse(element.Value, out parsed))
                    return parsed;

                return 0;
            }
            catch
            {
                return 0;
            }
        }
    }
}