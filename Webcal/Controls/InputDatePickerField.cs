namespace Webcal.Controls
{
    using System;
    using System.Windows;

    public class InputDatePickerField : BaseInputDatePickerField
    {
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof (string), typeof (InputDatePickerField), new PropertyMetadata(string.Empty));

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

            return Valid = SelectedDate != null;
        }

        public override void Clear()
        {
            Valid = true;
            SelectedDate = DateTime.Now;
        }
    }
}