namespace TachographReader.Controls
{
    using System.Collections;
    using System.Windows;
    using Xceed.Wpf.Toolkit;

    [TemplatePart(Name = "MultiSelectCombo", Type = typeof (InputMultiSelectComboField))]
    public class InputMultiSelectComboField : BaseInputField
    {
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof (string), typeof (InputMultiSelectComboField), new PropertyMetadata(null));

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof (IEnumerable), typeof (InputMultiSelectComboField), new PropertyMetadata(null));

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof (string), typeof (InputMultiSelectComboField), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IsTextSearchEnabledProperty =
            DependencyProperty.Register("IsTextSearchEnabled", typeof (bool), typeof (InputMultiSelectComboField), new PropertyMetadata(false));

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof (IEnumerable), typeof (InputMultiSelectComboField), new PropertyMetadata(null));

        private CheckComboBox _combo;

        static InputMultiSelectComboField()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (InputMultiSelectComboField), new FrameworkPropertyMetadata(typeof (InputMultiSelectComboField)));
        }

        public string Label
        {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public string DisplayMemberPath
        {
            get { return (string) GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }
        
        public bool IsTextSearchEnabled
        {
            get { return (bool) GetValue(IsTextSearchEnabledProperty); }
            set { SetValue(IsTextSearchEnabledProperty, value); }
        }

        public IEnumerable SelectedItems
        {
            get { return (IEnumerable) GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _combo = GetTemplateChild("Combo") as CheckComboBox;

            if (ValidateOnLoad)
            {
                IsValid();
            }
        }

        public override bool IsValid()
        {
            HasValidated = true;

            if (!IsMandatory)
            {
                return Valid = true;
            }

            return Valid = SelectedItems != null;
        }

        public override void Clear()
        {
            Valid = true;
            SelectedItems = null;

            if (_combo != null)
            {
                _combo.Text = null;
            }
        }
    }
}