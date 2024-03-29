﻿namespace TachographReader.Behaviours
{
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class TextChangedBehaviour
    {
        private static bool isThrottling = false;

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
            var control = target as TextBox;
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
            var control = sender as FrameworkElement;
            if (control == null)
            {
                return;
            }

            if (isThrottling)
            {
                return;
            }
            
            TaskScheduler synchronizationContext = TaskScheduler.FromCurrentSynchronizationContext();

            isThrottling = true;
            Task.Delay(500).ContinueWith(t =>
            {
                var command = (ICommand)control.GetValue(CommandProperty);
                command.Execute(control.GetValue(CommandParameterProperty));

                isThrottling = false;
            }, synchronizationContext);
        }

        public static DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof (ICommand), typeof (TextChangedBehaviour), new UIPropertyMetadata(CommandChanged));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof (object), typeof (TextChangedBehaviour));
    }
}