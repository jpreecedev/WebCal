namespace Webcal.Controls
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using Core;

    [BaseControl]
    public abstract class BaseInputField : UserControl, IValidate, INotifyPropertyChanged
    {
        public static readonly DependencyProperty IsMandatoryProperty =
            DependencyProperty.Register("IsMandatory", typeof (bool), typeof (BaseInputField), new PropertyMetadata(false));

        public static readonly DependencyProperty IsAutoPopulatedProperty =
            DependencyProperty.Register("IsAutoPopulated", typeof (bool), typeof (BaseInputField), new PropertyMetadata(false));

        public static readonly DependencyProperty IsHighlightedProperty =
            DependencyProperty.Register("IsHighlighted", typeof (bool), typeof (BaseInputField), new PropertyMetadata(false));

        protected BaseInputField()
        {
            Valid = true;
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
        public abstract void Clear();

        protected static void ReValidate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as BaseInputField;
            if (sender != null)
            {
                if (sender.HasValidated)
                    sender.IsValid();

                sender.ReValidated();
            }
        }

        protected virtual void ReValidated()
        {
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}