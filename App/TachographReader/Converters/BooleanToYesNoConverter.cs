namespace Webcal.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using Properties;

    [ValueConversion(typeof (bool), typeof (string))]
    public class BooleanToYesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is bool))
                return false;

            return (bool) value ? Resources.TXT_YES : Resources.TXT_NO;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}