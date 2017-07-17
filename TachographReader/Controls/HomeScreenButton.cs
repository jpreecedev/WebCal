namespace TachographReader.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    public class HomeScreenButton : Control
    {
        public static readonly DependencyProperty ClickCommandProperty =
            DependencyProperty.Register("ClickCommand", typeof(ICommand), typeof(HomeScreenButton), new PropertyMetadata(null));

        public static readonly DependencyProperty IsSmallSizeProperty =
            DependencyProperty.Register("IsSmallSize", typeof(bool), typeof(HomeScreenButton), new PropertyMetadata(false));

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(HomeScreenButton));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(HomeScreenButton), new PropertyMetadata(string.Empty));
        
        public static readonly DependencyProperty ClickCommandParameterProperty =
            DependencyProperty.Register("ClickCommandParameter", typeof(object), typeof(HomeScreenButton), new PropertyMetadata(null));
        
        public object ClickCommandParameter
        {
            get => (object) GetValue(ClickCommandParameterProperty);
            set => SetValue(ClickCommandParameterProperty, value);
        }
        
        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public ImageSource ImageSource
        {
            get => (ImageSource) GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public ICommand ClickCommand
        {
            get => (ICommand) GetValue(ClickCommandProperty);
            set => SetValue(ClickCommandProperty, value);
        }

        public bool IsSmallSize
        {
            get => (bool) GetValue(IsSmallSizeProperty);
            set => SetValue(IsSmallSizeProperty, value);
        }
    }
}