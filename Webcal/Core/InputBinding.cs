namespace Webcal.Core
{
    using System.Globalization;
    using System.Windows.Data;

    public class InputBinding : Binding
    {
        public InputBinding()
        {
            Configure();
        }

        public InputBinding(string path)
            : base(path)
        {
            Configure();
        }

        private void Configure()
        {
            Mode = BindingMode.TwoWay;
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            ConverterCulture = CultureInfo.CurrentUICulture;
        }
    }
}