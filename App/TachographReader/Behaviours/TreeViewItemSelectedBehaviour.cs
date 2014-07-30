using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Webcal.Behaviours
{
    public class TreeViewItemSelectedBehaviour
    {
        public static DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(TreeViewItemSelectedBehaviour), new UIPropertyMetadata(CommandChanged));

        public static void SetCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(CommandProperty, value);
        }

        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItem control = target as TreeViewItem;
            if (control != null)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    control.Selected += OnButtonClick;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    control.Selected -= OnButtonClick;
                }
            }
        }

        private static void OnButtonClick(object sender, RoutedEventArgs e)
        {
            TreeViewItem control = sender as TreeViewItem;
            if (control == null || !control.IsSelected)
                return;

            ICommand command = (ICommand)control.GetValue(CommandProperty);
            command.Execute(control.DataContext);
        }
    }
}
