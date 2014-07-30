using System.Windows;
using System.Windows.Input;

namespace Webcal.Behaviours
{
    public class LoadedBehaviour
    {
        public static DependencyProperty CommandProperty =
           DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(LoadedBehaviour), new UIPropertyMetadata(CommandChanged));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(LoadedBehaviour));

        public static object GetCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }

        public static void SetCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(CommandProperty, value);
        }

        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement control = target as FrameworkElement;
            if (control != null)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    ICommand command = (ICommand)control.GetValue(CommandProperty);
                    command.Execute(control.GetValue(CommandParameterProperty));
                }
            }
        }
    }
}
