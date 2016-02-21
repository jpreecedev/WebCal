namespace TachographReader.Controls
{
    using System.ComponentModel;
    using System.Data.SqlServerCe;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Core;

    [BaseControl]
    public class InputYesNoNotApplicableField : Control, IValidate, INotifyPropertyChanged
    {
        private RadioButton _noRadioButton;
        private RadioButton _notApplicableRadioButton;
        private RadioButton _yesRadioButton;

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof (bool?), typeof (InputYesNoNotApplicableField), new PropertyMetadata(false, ReValidate));

        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof (string), typeof (InputYesNoNotApplicableField), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof (string), typeof (InputYesNoNotApplicableField), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IsNotApplicableDisplayedProperty =
            DependencyProperty.Register("IsNotApplicableDisplayed", typeof (bool), typeof (InputYesNoNotApplicableField), new PropertyMetadata(true));

        public static readonly DependencyProperty IsMandatoryProperty =
            DependencyProperty.Register("IsMandatory", typeof (bool), typeof (InputYesNoNotApplicableField), new PropertyMetadata(false));
        
        public static readonly DependencyProperty IsHistoryModeProperty =
            DependencyProperty.Register("IsHistoryMode", typeof (bool), typeof (InputYesNoNotApplicableField), new PropertyMetadata(false));
        
        static InputYesNoNotApplicableField()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (InputYesNoNotApplicableField), new FrameworkPropertyMetadata(typeof (InputYesNoNotApplicableField)));
        }

        public InputYesNoNotApplicableField()
        {
            Valid = true;
            CheckChangedCommand = new DelegateCommand<RadioButton>(OnCheckChanged);
        }

        public bool IsHistoryMode
        {
            get { return (bool) GetValue(IsHistoryModeProperty); }
            set { SetValue(IsHistoryModeProperty, value); }
        }

        public bool IsMandatory
        {
            get { return (bool) GetValue(IsMandatoryProperty); }
            set { SetValue(IsMandatoryProperty, value); }
        }

        public string Label
        {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public bool? Value
        {
            get { return (bool?) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public string GroupName
        {
            get { return (string) GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        public bool IsNotApplicableDisplayed
        {
            get { return (bool) GetValue(IsNotApplicableDisplayedProperty); }
            set { SetValue(IsNotApplicableDisplayedProperty, value); }
        }

        public bool Valid { get; set; }

        protected bool HasValidated { get; set; }

        public ICommand CheckChangedCommand { get; set; }

        public bool HasSelectedValue { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsValid()
        {
            HasValidated = true;

            if (IsHistoryMode)
            {
                return true;
            }

            if (!IsNotApplicableDisplayed)
            {
                return Valid = HasSelectedValue;
            }

            return Valid = Value.HasValue || HasSelectedValue;
        }
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _yesRadioButton = GetTemplateChild("YesRadioButton") as RadioButton;
            _noRadioButton = GetTemplateChild("NoRadioButton") as RadioButton;
            _notApplicableRadioButton = GetTemplateChild("NotApplicableRadioButton") as RadioButton;

            if (!IsHistoryMode)
                return;

            switch (Value)
            {
                case true:
                    if (_yesRadioButton != null) _yesRadioButton.IsChecked = true;
                    break;
                case false:
                    if (_noRadioButton != null) _noRadioButton.IsChecked = true;
                    break;
                default:
                    if (_notApplicableRadioButton != null) _notApplicableRadioButton.IsChecked = true;
                    break;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static void ReValidate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as InputYesNoNotApplicableField;
            if (sender != null && sender.HasValidated)
            {
                sender.IsValid();
            }
        }

        private void OnCheckChanged(RadioButton radioButton)
        {
            if (radioButton == null)
            {
                return;
            }

            HasSelectedValue = true;

            switch (radioButton.CurrentValue)
            {
                case 1:
                    Value = true;
                    break;
                case 2:
                    Value = false;
                    break;
                default:
                    Value = null;
                    break;
            }

            ReValidate(this, new DependencyPropertyChangedEventArgs());
        }
    }
}