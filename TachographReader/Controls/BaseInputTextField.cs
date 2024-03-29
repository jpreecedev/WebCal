﻿namespace TachographReader.Controls
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
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

        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof (bool), typeof (BaseInputTextField));

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
        
        public bool IsLoading
        {
            get { return (bool) GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}