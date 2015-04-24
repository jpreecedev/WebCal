namespace TachographReader.Controls
{
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class InputAutoCompleteField : BaseInputField
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof (IEnumerable), typeof (InputAutoCompleteField), new PropertyMetadata(null));
        
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof (string), typeof (InputAutoCompleteField), new PropertyMetadata(null));

        public static readonly DependencyProperty TextChangedCommandProperty =
            DependencyProperty.Register("TextChangedCommand", typeof (ICommand), typeof (InputAutoCompleteField), new PropertyMetadata(null, OnTextCommandChanged));

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof (DataTemplate), typeof (InputAutoCompleteField), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectionChangedCommandProperty =
            DependencyProperty.Register("SelectionChangedCommand", typeof(ICommand), typeof(InputAutoCompleteField), new PropertyMetadata(null, OnSelectionChanged));

        private AutoCompleteBox _autoCompleteBox;

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate) GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public ICommand TextChangedCommand
        {
            get { return (ICommand) GetValue(TextChangedCommandProperty); }
            set { SetValue(TextChangedCommandProperty, value); }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public string Label
        {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public ICommand SelectionChangedCommand
        {
            get { return (ICommand) GetValue(SelectionChangedCommandProperty); }
            set { SetValue(SelectionChangedCommandProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _autoCompleteBox = GetTemplateChild("AutoCompleteBox") as AutoCompleteBox;
        }

        public override bool IsValid()
        {
            return _autoCompleteBox.SelectedItem != null;
        }

        public override void Clear()
        {
            if (_autoCompleteBox != null)
            {
                _autoCompleteBox.SelectedItem = null;
                _autoCompleteBox.ItemsSource = null;
            }
        }

        private static void OnTextCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as InputAutoCompleteField;
            if (control != null)
            {
                if (control.TextChangedCommand != null && control._autoCompleteBox != null)
                {
                    control._autoCompleteBox.TextChanged += (sender, args) => { control.TextChangedCommand.Execute(control._autoCompleteBox.Text); };
                    control._autoCompleteBox.LostFocus += (sender, args) => { control.TextChangedCommand.Execute(control._autoCompleteBox.Text); };
                }
            }
        }

        private static void OnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as InputAutoCompleteField;
            if (control != null)
            {
                if (control.SelectionChangedCommand != null && control._autoCompleteBox != null)
                {
                    control._autoCompleteBox.SelectionChanged += (sender, args) => { control.SelectionChangedCommand.Execute(control._autoCompleteBox.SelectedItem); };
                }
            }
        }
    }
}