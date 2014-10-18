namespace Webcal.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    [ValueConversion(typeof (bool), typeof (Visibility))]
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                return value;
            }

            var b = (bool) value;
            return b ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}