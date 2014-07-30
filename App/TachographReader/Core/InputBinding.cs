using System.Windows.Data;

namespace Webcal.Core
{
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
        }
    }
}
