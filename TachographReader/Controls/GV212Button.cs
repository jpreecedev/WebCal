namespace TachographReader.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class GV212Button : Control
    {
        public static readonly DependencyProperty IsOutOfDateProperty = DependencyProperty.Register("IsOutOfDate", typeof(bool), typeof(GV212Button));

        public static readonly DependencyProperty ClickCommandProperty =
            DependencyProperty.Register("ClickCommand", typeof(ICommand), typeof(GV212Button), new PropertyMetadata(null));

        public static readonly DependencyProperty IsSmallSizeProperty =
            DependencyProperty.Register("IsSmallSize", typeof(bool), typeof(GV212Button), new PropertyMetadata(false));

        public bool IsOutOfDate
        {
            get { return (bool) GetValue(IsOutOfDateProperty); }
            set { SetValue(IsOutOfDateProperty, value); }
        }

        public ICommand ClickCommand
        {
            get { return (ICommand) GetValue(ClickCommandProperty); }
            set { SetValue(ClickCommandProperty, value); }
        }

        public bool IsSmallSize
        {
            get { return (bool) GetValue(IsSmallSizeProperty); }
            set { SetValue(IsSmallSizeProperty, value); }
        }
    }
}