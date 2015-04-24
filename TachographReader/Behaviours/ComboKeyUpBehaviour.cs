namespace TachographReader.Behaviours
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Controls;

    public class ComboKeyUpBehaviour
    {
        public static DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof (ICommand), typeof (ComboKeyUpBehaviour), new UIPropertyMetadata(CommandChanged));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(ComboKeyUpBehaviour));

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
            var control = target as InputComboField;
            if (control != null && control.Combo != null)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    control.Combo.KeyUp += (sender, args) => { OnKeyUp(control); };
                }
            }
        }

        private static void OnKeyUp(DependencyObject inputComboField)
        {
            if (inputComboField == null)
            {
                return;
            }

            var command = (ICommand)inputComboField.GetValue(CommandProperty);
            command.Execute(inputComboField.GetValue(CommandParameterProperty));
        }
    }
}