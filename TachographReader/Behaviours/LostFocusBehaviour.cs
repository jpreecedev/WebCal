namespace TachographReader.Behaviours
{
    using System.Windows;
    using System.Windows.Input;

    public class LostFocusBehaviour
    {
        public static void SetCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(CommandProperty, value);
        }

        public static object GetCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }

        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var control = target as UIElement;
            if (control != null)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    control.LostFocus += OnLostFocus;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    control.LostFocus -= OnLostFocus;
                }
            }
        }

        private static void OnLostFocus(object sender, RoutedEventArgs e)
        {
            var control = sender as UIElement;
            if (control == null)
            {
                return;
            }

            var command = (ICommand) control.GetValue(CommandProperty);
            command.Execute(control.GetValue(CommandParameterProperty));
        }

        public static DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof (ICommand), typeof (LostFocusBehaviour), new UIPropertyMetadata(CommandChanged));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof (object), typeof (LostFocusBehaviour));
    }
}