using System.Windows;
using System.Windows.Input;

namespace Webcal.Behaviours
{
    public class LostFocusBehaviour
    {
        public static DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(LostFocusBehaviour), new UIPropertyMetadata(CommandChanged));

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

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(LostFocusBehaviour));

        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            UIElement control = target as UIElement;
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
            UIElement control = sender as UIElement;
            if (control == null)
                return;

            ICommand command = (ICommand)control.GetValue(CommandProperty);
            command.Execute(control.GetValue(CommandParameterProperty));
        }
    }
}
