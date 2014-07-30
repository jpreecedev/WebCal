using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Webcal.Behaviours
{
    public class TextChangedBehaviour
    {
        public static DependencyProperty CommandProperty =
      DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(TextChangedBehaviour), new UIPropertyMetadata(CommandChanged));

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
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(TextChangedBehaviour));

        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            TextBox control = target as TextBox;
            if (control != null)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    control.TextChanged += OnTextChanged;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    control.TextChanged -= OnTextChanged;
                }
            }
        }

        private static void OnTextChanged(object sender, RoutedEventArgs e)
        {
            FrameworkElement control = sender as FrameworkElement;
            if (control == null)
                return;

            ICommand command = (ICommand)control.GetValue(CommandProperty);
            command.Execute(control.GetValue(CommandParameterProperty));
        }
    }
}
