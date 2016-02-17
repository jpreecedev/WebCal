namespace TachographReader.Controls
{
    using System.Windows;
    using System.Windows.Input;
    using Behaviours;

    public class RadioButton : System.Windows.Controls.RadioButton
    {
        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register("CurrentValue", typeof(int), typeof(RadioButton), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty CheckedValueProperty =
            DependencyProperty.Register("CheckedValue", typeof(int), typeof(RadioButton), new UIPropertyMetadata(0));

        public int CurrentValue
        {
            get { return (int)GetValue(CurrentValueProperty); }
            set { SetValue(CurrentValueProperty, value); }
        }

        public int CheckedValue
        {
            get { return (int)GetValue(CheckedValueProperty); }
            set { SetValue(CheckedValueProperty, value); }
        }

        protected override void OnClick()
        {
            base.OnClick();

            SetValue(CurrentValueProperty, CheckedValue);
            
            if (Command != null)
            {
                Command.Execute(this);
            }
        }
    }
}