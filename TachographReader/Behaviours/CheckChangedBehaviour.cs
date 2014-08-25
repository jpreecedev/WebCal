namespace Webcal.Behaviours
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class CheckChangedBehaviour
    {
        public static DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof (ICommand), typeof (CheckChangedBehaviour), new UIPropertyMetadata(CommandChanged));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof (object), typeof (CheckChangedBehaviour));

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
            var control = target as CheckBox;
            if (control != null)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    control.Checked += OnCheckChanged;
                    control.Unchecked += OnCheckChanged;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    control.Checked -= OnCheckChanged;
                    control.Unchecked -= OnCheckChanged;
                }
            }
        }

        private static void OnCheckChanged(object sender, RoutedEventArgs e)
        {
            var control = sender as FrameworkElement;
            if (control == null)
                return;

            var command = (ICommand) control.GetValue(CommandProperty);
            command.Execute(control.GetValue(CommandParameterProperty));
        }
    }
}