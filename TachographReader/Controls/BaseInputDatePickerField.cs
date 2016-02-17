namespace TachographReader.Controls
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using Core;

    [BaseControl]
    public abstract class BaseInputDatePickerField : DatePicker, IValidate, INotifyPropertyChanged
    {
        public static readonly DependencyProperty IsMandatoryProperty =
            DependencyProperty.Register("IsMandatory", typeof (bool), typeof (BaseInputDatePickerField), new PropertyMetadata(false));

        public static readonly DependencyProperty IsAutoPopulatedProperty =
            DependencyProperty.Register("IsAutoPopulated", typeof (bool), typeof (BaseInputDatePickerField), new PropertyMetadata(false));

        public static readonly DependencyProperty IsHighlightedProperty =
            DependencyProperty.Register("IsHighlighted", typeof (bool), typeof (BaseInputDatePickerField), new PropertyMetadata(false));

        protected BaseInputDatePickerField()
        {
            Valid = true;
            SelectedDateChanged += (sender, e) => ReValidate(this, new DependencyPropertyChangedEventArgs());
        }

        public bool Valid { get; set; }
        protected bool HasValidated { get; set; }

        public bool IsMandatory
        {
            get { return (bool) GetValue(IsMandatoryProperty); }
            set { SetValue(IsMandatoryProperty, value); }
        }

        public bool IsAutoPopulated
        {
            get { return (bool) GetValue(IsAutoPopulatedProperty); }
            set { SetValue(IsAutoPopulatedProperty, value); }
        }

        public bool IsHighlighted
        {
            get { return (bool) GetValue(IsHighlightedProperty); }
            set { SetValue(IsHighlightedProperty, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public abstract bool IsValid();

        protected static void ReValidate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as BaseInputDatePickerField;
            if (sender != null && sender.HasValidated)
            {
                sender.IsValid();
            }
        }

        public abstract void Clear();

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}