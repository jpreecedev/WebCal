namespace TachographReader.Controls
{
    using System;
    using System.Linq.Expressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;

    public class InputTextField : BaseInputTextField
    {
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof (string), typeof (InputTextField), new PropertyMetadata(null));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof (ICommand), typeof (InputTextField));

        public static readonly DependencyProperty CommandLabelProperty =
            DependencyProperty.Register("CommandLabel", typeof (string), typeof (InputTextField));

        public static readonly DependencyProperty IsMultilineProperty =
            DependencyProperty.Register("IsMultiline", typeof (bool), typeof (InputTextField), new PropertyMetadata(false));

        public static readonly DependencyProperty IsLabelAutoWidthProperty =
            DependencyProperty.Register("IsLabelAutoWidth", typeof (bool), typeof (InputTextField), new PropertyMetadata(false));

        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register("LabelWidth", typeof (int), typeof (InputTextField), new PropertyMetadata(0));

        public static readonly DependencyProperty IsLabelCustomWidthProperty =
            DependencyProperty.Register("IsLabelCustomWidth", typeof (bool), typeof (InputTextField), new PropertyMetadata(false));

        static InputTextField()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (InputTextField), new FrameworkPropertyMetadata(typeof (InputTextField)));
        }

        public InputTextField()
        {
            AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(SelectivelyIgnoreMouseButton), true);
            AddHandler(GotKeyboardFocusEvent, new RoutedEventHandler(SelectAllText), true);
            AddHandler(MouseDoubleClickEvent, new RoutedEventHandler(SelectAllText), true);
        }

        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public string CommandLabel
        {
            get { return (string) GetValue(CommandLabelProperty); }
            set { SetValue(CommandLabelProperty, value); }
        }

        public string Label
        {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public bool IsMultiline
        {
            get { return (bool) GetValue(IsMultilineProperty); }
            set { SetValue(IsMultilineProperty, value); }
        }

        public bool IsLabelAutoWidth
        {
            get { return (bool) GetValue(IsLabelAutoWidthProperty); }
            set { SetValue(IsLabelAutoWidthProperty, value); }
        }

        public int LabelWidth
        {
            get { return (int) GetValue(LabelWidthProperty); }
            set { SetValue(LabelWidthProperty, value); }
        }
        
        public bool IsLabelCustomWidth
        {
            get { return (bool) GetValue(IsLabelCustomWidthProperty); }
            set { SetValue(IsLabelCustomWidthProperty, value); }
        }
        
        public override bool IsValid()
        {
            HasValidated = true;

            if (!IsMandatory)
            {
                return Valid = true;
            }

            return Valid = !string.IsNullOrEmpty(Text);
        }

        public override void OnClear()
        {
            Clear();
            Valid = true;
        }

        public static InputTextField CreateInputTextField<TProperty, TBinding>(string label, Expression<Func<TProperty>> property, object source, ICommand command = null, string commandLabel = null, bool readOnly = false) where TBinding : Binding, new()
        {
            var textField = new InputTextField
            {
                Label = label,
                Command = command,
                CommandLabel = commandLabel,
                IsReadOnly = readOnly
            };

            var binding = new TBinding
            {
                Source = source,
                Path = new PropertyPath(((MemberExpression) property.Body).Member.Name)
            };

            textField.SetBinding(TextProperty, binding);

            return textField;
        }

        private static void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            if (e.Source != null && e.Source.GetType().IsAssignableFrom(typeof (Button)))
            {
                return;
            }

            // Find the TextBox
            DependencyObject parent = e.OriginalSource as UIElement;
            while (parent != null && !(parent is TextBox))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            if (parent != null)
            {
                var textBox = (TextBox) parent;
                if (!textBox.IsKeyboardFocusWithin)
                {
                    textBox.Focus();
                    e.Handled = true;
                }
            }
        }

        private static void SelectAllText(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                textBox.SelectAll();
            }
        }
    }
}