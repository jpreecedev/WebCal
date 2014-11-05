namespace Webcal.Controls
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;

    public class InputNumberPicker : BaseInputTextField
    {
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof (string), typeof (InputNumberPicker), new PropertyMetadata(null));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof (int), typeof (InputNumberPicker), new PropertyMetadata(10));

        static InputNumberPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (InputNumberPicker), new FrameworkPropertyMetadata(typeof (InputNumberPicker)));
        }

        public string Label
        {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public int Maximum
        {
            get { return (int) GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public override void OnClear()
        {
            Clear();
            Valid = true;
        }

        protected override void ReValidate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as BaseInputTextField;
            if (sender != null)
            {
                sender.IsValid();
            }
        }

        public override bool IsValid()
        {
            HasValidated = true;

            if (string.IsNullOrEmpty(Text))
            {
                return false;
            }

            if (Text.ToCharArray().Any(c => !Char.IsNumber(c)))
            {
                Text = Regex.Replace(Text, "[^.0-9]", string.Empty);
            }

            int number;
            if (int.TryParse(Text, out number))
            {
                if (number > Maximum)
                {
                    return Valid = false;
                }

                return Valid = number > 0;
            }

            return Valid = !string.IsNullOrEmpty(Text);
        }
    }
}