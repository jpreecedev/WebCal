using System;
using System.Windows;

namespace Webcal.Controls
{
    public class InputDatePickerField : BaseInputDatePickerField
    {
        #region Dependency Properties

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(InputDatePickerField), new PropertyMetadata(string.Empty));

        #endregion

        #region Overrides

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

        #endregion
    }
}
