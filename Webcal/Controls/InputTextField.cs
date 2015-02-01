namespace Webcal.Controls
{
    using System.Windows;
    using System.Windows.Input;

    public class InputTextField : BaseInputTextField
    {
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof (string), typeof (InputTextField), new PropertyMetadata(null));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof (ICommand), typeof (InputTextField));

        public static readonly DependencyProperty CommandLabelProperty =
            DependencyProperty.Register("CommandLabel", typeof (string), typeof (InputTextField));

        static InputTextField()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (InputTextField), new FrameworkPropertyMetadata(typeof (InputTextField)));
        }
        
        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        
        public string CommandLabel
        {
            get { return (string) GetValue(CommandLabelProperty); }
            set { SetValue(CommandLabelProperty, value); }
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
            {
                return Valid = true;
            }

            return Valid = !string.IsNullOrEmpty(Text);
        }

        public override void OnClear()
        {
            Clear();
            Valid = true;
        }
    }
}