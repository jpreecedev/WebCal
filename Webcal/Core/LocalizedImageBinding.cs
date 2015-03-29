namespace Webcal.Core
{
    using System.Globalization;
    using System.Windows.Data;
    using Converters;

    public class LocalizedImageBinding : Binding
    {
        public LocalizedImageBinding()
        {
            Configure();
        }

        public LocalizedImageBinding(string path)
            : base(path)
        {
            Configure();
        }

        private void Configure()
        {
            ConverterCulture = CultureInfo.CurrentUICulture;
            Converter = new LocalizedImageConverter();
        }
    }
}