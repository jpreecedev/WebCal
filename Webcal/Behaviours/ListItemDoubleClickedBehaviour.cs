namespace TachographReader.Behaviours
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class ListItemDoubleClickedBehaviour
    {
        public static void SetCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(CommandProperty, value);
        }

        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var control = target as ListView;
            if (control != null)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    control.MouseDoubleClick += OnDoubleClick;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    control.MouseDoubleClick -= OnDoubleClick;
                }
            }
        }

        private static void OnDoubleClick(object sender, RoutedEventArgs e)
        {
            var control = sender as ListView;
            if (control == null)
            {
                return;
            }

            var command = (ICommand) control.GetValue(CommandProperty);
            command.Execute(null);
        }

        public static DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof (ICommand), typeof (ListItemDoubleClickedBehaviour), new UIPropertyMetadata(CommandChanged));
    }
}