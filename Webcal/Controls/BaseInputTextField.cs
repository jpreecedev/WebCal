namespace Webcal.Controls
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using Core;

    [BaseControl]
    public abstract class BaseInputTextField : TextBox, IValidate, INotifyPropertyChanged
    {
        public static readonly DependencyProperty IsMandatoryProperty =
            DependencyProperty.Register("IsMandatory", typeof (bool), typeof (BaseInputTextField), new PropertyMetadata(false));

        public static readonly DependencyProperty IsAutoPopulatedProperty =
            DependencyProperty.Register("IsAutoPopulated", typeof (bool), typeof (BaseInputTextField), new PropertyMetadata(false));

        public static readonly DependencyProperty IsHighlightedProperty =
            DependencyProperty.Register("IsHighlighted", typeof (bool), typeof (BaseInputTextField), new PropertyMetadata(false));

        protected BaseInputTextField()
        {
            Valid = true;
            TextChanged += (sender, e) => ReValidate(this, new DependencyPropertyChangedEventArgs());
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
        public abstract void OnClear();

        protected virtual void ReValidate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as BaseInputTextField;
            if (sender != null && sender.HasValidated)
            {
                sender.IsValid();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}