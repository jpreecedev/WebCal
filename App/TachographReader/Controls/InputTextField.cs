namespace Webcal.Controls
{
    using System.Windows;

    public class InputTextField : BaseInputTextField
    {
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof (string), typeof (InputTextField), new PropertyMetadata(null));

        static InputTextField()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (InputTextField), new FrameworkPropertyMetadata(typeof (InputTextField)));
        }
        
        public string Label
        {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        
        public override bool IsValid()
        {
            HasValidated = true;

            if (!IsMandatory)
                return Valid = true;

            return Valid = !string.IsNullOrEmpty(Text);
        }

        public override void OnClear()
        {
            Clear();
            Valid = true;
        }
    }
}