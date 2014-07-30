using System.Windows;

namespace Webcal.Controls
{
    public class InputTextField : BaseInputTextField
    {
        #region Constructor

        static InputTextField()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InputTextField), new FrameworkPropertyMetadata(typeof(InputTextField)));
        }

        #endregion

        #region Dependency Properties

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(InputTextField), new PropertyMetadata(null));

        #endregion

        #region Overrides

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

        #endregion
    }
}
