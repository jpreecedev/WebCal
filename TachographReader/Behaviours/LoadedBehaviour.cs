﻿namespace TachographReader.Behaviours
{
    using System.Windows;
    using System.Windows.Input;

    public class LoadedBehaviour
    {
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
            var control = target as FrameworkElement;
            if (control != null)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    var command = (ICommand) control.GetValue(CommandProperty);
                    command.Execute(control.GetValue(CommandParameterProperty));
                }
            }
        }

        public static DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof (ICommand), typeof (LoadedBehaviour), new UIPropertyMetadata(CommandChanged));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof (object), typeof (LoadedBehaviour));
    }
}