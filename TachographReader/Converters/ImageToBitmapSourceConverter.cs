namespace Webcal.Converters
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;
    using Library;

    [ValueConversion(typeof (Image), typeof (BitmapSource))]
    public class ImageToBitmapSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Image))
                return null;

            return ((Image) value).ToBitmapSource();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}