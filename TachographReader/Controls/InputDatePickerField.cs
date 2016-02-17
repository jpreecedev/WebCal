namespace TachographReader.Controls
{
    using System;
    using System.Windows;

    public class InputDatePickerField : BaseInputDatePickerField
    {
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof (string), typeof (InputDatePickerField), new PropertyMetadata(string.Empty));
        
        public static readonly DependencyProperty IsLabelWidthCustomProperty =
            DependencyProperty.Register("IsLabelWidthCustom", typeof(bool), typeof(InputDatePickerField), new PropertyMetadata(false));

        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register("LabelWidth", typeof(int), typeof(InputDatePickerField), new PropertyMetadata(0));
        
        public string Label
        {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public bool IsLabelWidthCustom
        {
            get { return (bool)GetValue(IsLabelWidthCustomProperty); }
            set { SetValue(IsLabelWidthCustomProperty, value); }
        }
        
        public int LabelWidth
        {
            get { return (int)GetValue(LabelWidthProperty); }
            set { SetValue(LabelWidthProperty, value); }
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