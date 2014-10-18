namespace Webcal.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;

    public class ReadOnlyMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2)
            {
                return false;
            }
            if (values.Any(v => v == DependencyProperty.UnsetValue))
            {
                return false;
            }

            var isReadOnly = (bool) values[0];
            if (!isReadOnly)
            {
                return true;
            }

            object val = values[1];
            return val == null || string.IsNullOrEmpty(val.ToString());
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}