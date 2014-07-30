using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Webcal.Converters
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class InverseNullToVisibilityConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
