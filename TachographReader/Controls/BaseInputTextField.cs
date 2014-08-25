using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Webcal.Core;

namespace Webcal.Controls
{
    [BaseControl]
    public abstract class BaseInputTextField : TextBox, IValidate, INotifyPropertyChanged
    {
        
        protected BaseInputTextField()
        {
            Valid = true;
            TextChanged += (sender, e) => ReValidate(this, new DependencyPropertyChangedEventArgs());
        }

        
        
        public bool Valid { get; set; }

        
        
        public abstract void OnClear();

        public abstract bool IsValid();

        
        
        protected bool HasValidated { get; set; }

        protected static void ReValidate(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BaseInputTextField sender = d as BaseInputTextField;
            if (sender != null && sender.HasValidated)
            {
                sender.IsValid();
            }
        }

        
        
        public bool IsMandatory
        {
            get { return (bool)GetValue(IsMandatoryProperty); }
            set { SetValue(IsMandatoryProperty, value); }
        }

        public static readonly DependencyProperty IsMandatoryProperty =
            DependencyProperty.Register("IsMandatory", typeof(bool), typeof(BaseInputTextField), new PropertyMetadata(false));

        public bool IsAutoPopulated
        {
            get { return (bool)GetValue(IsAutoPopulatedProperty); }
            set { SetValue(IsAutoPopulatedProperty, value); }
        }

        public static readonly DependencyProperty IsAutoPopulatedProperty =
            DependencyProperty.Register("IsAutoPopulated", typeof(bool), typeof(BaseInputTextField), new PropertyMetadata(false));

        public bool IsHighlighted
        {
            get { return (bool)GetValue(IsHighlightedProperty); }
            set { SetValue(IsHighlightedProperty, value); }
        }

        public static readonly DependencyProperty IsHighlightedProperty =
            DependencyProperty.Register("IsHighlighted", typeof(bool), typeof(BaseInputTextField), new PropertyMetadata(false));

        
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

            }
}
