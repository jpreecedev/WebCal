namespace TachographReader.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    [TemplatePart(Name = "Combo", Type = typeof(InputComboField))]
    public class InputComboField : BaseInputField
    {
        public static readonly DependencyProperty IsSelectedItemMandatoryProperty =
            DependencyProperty.Register("IsSelectedItemMandatory", typeof(bool), typeof(InputComboField), new PropertyMetadata(false));

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(InputComboField), new PropertyMetadata(null));

        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(InputComboField), new PropertyMetadata(true));

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(InputComboField), new PropertyMetadata(null));

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(InputComboField), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty SelectedTextProperty =
            DependencyProperty.Register("SelectedText", typeof(string), typeof(InputComboField), new PropertyMetadata(null, ReValidate));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(InputComboField), new PropertyMetadata(null, SelectedItemChanged));

        public static readonly DependencyProperty IsSynchronisedWithCurrentItemProperty =
            DependencyProperty.Register("IsSynchronisedWithCurrentItem", typeof(bool), typeof(InputComboField), new PropertyMetadata(false));

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(InputComboField), new PropertyMetadata(-1));

        public static readonly DependencyProperty SelectedTextChangedProperty =
            DependencyProperty.Register("SelectedTextChanged", typeof(ICommand), typeof(InputComboField));

        public static readonly DependencyProperty IsTextSearchEnabledProperty =
            DependencyProperty.Register("IsTextSearchEnabled", typeof(bool), typeof(InputComboField), new PropertyMetadata(true));

        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register("LabelWidth", typeof(int), typeof(InputComboField), new PropertyMetadata(0));

        public static readonly DependencyProperty IsLabelCustomWidthProperty =
            DependencyProperty.Register("IsLabelCustomWidth", typeof(bool), typeof(InputComboField), new PropertyMetadata(false));

        static InputComboField()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InputComboField), new FrameworkPropertyMetadata(typeof(InputComboField)));
        }
        
        public bool IsSelectedItemMandatory
        {
            get { return (bool) GetValue(IsSelectedItemMandatoryProperty); }
            set { SetValue(IsSelectedItemMandatoryProperty, value); }
        }
        
        public string Label
        {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public bool IsEditable
        {
            get { return (bool) GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
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

        public string SelectedText
        {
            get { return (string) GetValue(SelectedTextProperty); }
            set { SetValue(SelectedTextProperty, value); }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public bool IsSynchronisedWithCurrentItem
        {
            get { return (bool) GetValue(IsSynchronisedWithCurrentItemProperty); }
            set { SetValue(IsSynchronisedWithCurrentItemProperty, value); }
        }

        public int SelectedIndex
        {
            get { return (int) GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public ICommand SelectedTextChanged
        {
            get { return (ICommand) GetValue(SelectedTextChangedProperty); }
            set { SetValue(SelectedTextChangedProperty, value); }
        }

        public bool IsTextSearchEnabled
        {
            get { return (bool) GetValue(IsTextSearchEnabledProperty); }
            set { SetValue(IsTextSearchEnabledProperty, value); }
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

        public ComboBox Combo { get; private set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Combo = GetTemplateChild("Combo") as ComboBox;

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

            return Valid = !string.IsNullOrEmpty(SelectedText) && (!IsSelectedItemMandatory || SelectedItem != null);
        }

        public override void Clear()
        {
            Valid = true;
            SelectedText = string.Empty;

            if (Combo != null)
            {
                Combo.Text = null;
            }
        }

        protected override void ReValidated()
        {
            base.ReValidated();

            //BLATENT LAST MINUTE HACK
            var source = ItemsSource as IList<string>;

            if (source != null && source.Any(t => t == SelectedText))
            {
                SelectedIndex = source.IndexOf(SelectedText);
            }
        }

        private static void SelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as InputComboField;
            if (control != null)
            {
                if (e.NewValue == null)
                {
                    control.SelectedText = null;
                }
                else
                {
                    control.SelectedText = e.NewValue.ToString();
                }

                if (control.SelectedTextChanged != null)
                {
                    control.SelectedTextChanged.Execute(control.SelectedText);
                }
            }
        }
    }
}