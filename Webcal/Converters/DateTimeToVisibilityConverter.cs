namespace Webcal.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    [ValueConversion(typeof (DateTime), typeof (Visibility))]
    public class DateTimeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime))
            {
                return Visibility.Collapsed;
            }

            var dateTime = (DateTime) value;
            if (dateTime == default(DateTime))
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}