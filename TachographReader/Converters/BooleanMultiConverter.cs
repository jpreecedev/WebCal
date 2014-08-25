namespace Webcal.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    public class BooleanMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return null;

            return values.OfType<bool>().All(v => v);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}