namespace TachographReader.Core
{
    using System.Globalization;
    using System.Windows.Data;

    public class OneWayInputBinding : Binding
    {
        public OneWayInputBinding()
        {
            Configure();
        }

        public OneWayInputBinding(string path)
            : base(path)
        {
            Configure();
        }

        private void Configure()
        {
            Mode = BindingMode.OneWay;
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            ConverterCulture = CultureInfo.CurrentUICulture;
        }
    }
}