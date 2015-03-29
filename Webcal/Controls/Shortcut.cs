namespace TachographReader.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Views;

    public class Shortcut : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof (string), typeof (Shortcut), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof (ICommand), typeof (Shortcut), new PropertyMetadata(null));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof (object), typeof (Shortcut), new PropertyMetadata(null));

        public static readonly DependencyProperty SettingsViewModelProperty =
            DependencyProperty.Register("SettingsViewModel", typeof (SettingsViewModel), typeof (Shortcut), new PropertyMetadata(null));

        public static readonly DependencyProperty ViewProperty =
            DependencyProperty.Register("View", typeof (Type), typeof (Shortcut), new PropertyMetadata(null));

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public SettingsViewModel SettingsViewModel
        {
            get { return (SettingsViewModel) GetValue(SettingsViewModelProperty); }
            set { SetValue(SettingsViewModelProperty, value); }
        }

        public Type View
        {
            get { return (Type) GetValue(ViewProperty); }
            set { SetValue(ViewProperty, value); }
        }
    }
}