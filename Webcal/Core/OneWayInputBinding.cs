namespace Webcal.Core
{
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
        }
    }
}