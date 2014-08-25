namespace Webcal.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Core;
    using Views;

    [TemplatePart(Name = "PART_TextBox", Type = typeof (TextBox))]
    public class PopupWindow : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty PromptProperty =
            DependencyProperty.Register("Prompt", typeof (string), typeof (PopupWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof (SettingsViewModel), typeof (PopupWindow),
                new PropertyMetadata(null));

        private TextBox _textBox;

        public PopupWindow()
        {
            OKCommand = new DelegateCommand<object>(OnOK);
            CancelCommand = new DelegateCommand<object>(OnCancel);

            IsVisibleChanged += VisibilityChanged;
        }

        public string Prompt
        {
            get { return (string) GetValue(PromptProperty); }
            set { SetValue(PromptProperty, value); }
        }

        public SettingsViewModel ViewModel
        {
            get { return (SettingsViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }


        public DelegateCommand<object> OKCommand { get; set; }

        public DelegateCommand<object> CancelCommand { get; set; }

        public string Text { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _textBox = GetTemplateChild("PART_TextBox") as TextBox;
        }
        
        private void OnCancel(object obj)
        {
            if (ViewModel != null)
                ViewModel.IsPromptVisible = false;
        }

        private void OnOK(object obj)
        {
            if (ViewModel == null) return;

            ViewModel.IsPromptVisible = false;

            if (ViewModel.Callback != null)
                ViewModel.Callback.Invoke((string) obj);
        }

        private void VisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool) e.NewValue)
            {
                Text = string.Empty;

                if (_textBox != null)
                    Dispatcher.BeginInvoke(new Action(() => FocusManager.SetFocusedElement(this, _textBox)), DispatcherPriority.Render);
            }
        }
        
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}