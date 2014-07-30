using System;
using System.Globalization;
using System.Windows.Data;
using Webcal.Properties;

namespace Webcal.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class BooleanToYesNoConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is bool))
                return false;

            return (bool)value ? Resources.TXT_YES : Resources.TXT_NO;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
