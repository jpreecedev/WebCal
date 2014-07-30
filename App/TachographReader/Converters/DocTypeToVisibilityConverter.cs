using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Webcal.Properties;

namespace Webcal.Converters
{
    [ValueConversion(typeof (string), typeof (Visibility))]
    public class DocTypeToVisibilityConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty((string)value))
                return Visibility.Visible;

            return (string)value == Resources.TXT_MINOR_WORK_DETAILS ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}