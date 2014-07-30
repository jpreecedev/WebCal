using System.Windows;
using System.Windows.Input;

namespace Webcal.Behaviours
{
    public class ElementClickedBehaviour
    {
        public static DependencyProperty CommandProperty =
          DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(ElementClickedBehaviour), new UIPropertyMetadata(CommandChanged));

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
                    control.MouseLeftButtonDown += OnButtonClick;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    control.MouseLeftButtonDown -= OnButtonClick;
                }
            }
        }

        private static void OnButtonClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement control = sender as FrameworkElement;
            if (control == null)
                return;

            ICommand command = (ICommand)control.GetValue(CommandProperty);
            command.Execute(null);
        }
    }
}
