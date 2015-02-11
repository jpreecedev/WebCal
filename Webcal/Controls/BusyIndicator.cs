namespace Webcal.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    public class BusyIndicator : Control
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof (string), typeof (BusyIndicator), new UIPropertyMetadata(null));

        static BusyIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (BusyIndicator), new FrameworkPropertyMetadata(typeof (BusyIndicator)));
        }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}