using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Webcal.Core;
using Webcal.Views;

namespace Webcal.Controls
{
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    public class PopupWindow : UserControl, INotifyPropertyChanged
    {
        #region Private Members

        private TextBox _textBox;

        #endregion

        #region Constructor

        public PopupWindow()
        {
            OKCommand = new DelegateCommand<object>(OnOK);
            CancelCommand = new DelegateCommand<object>(OnCancel);

            IsVisibleChanged += VisibilityChanged;
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty PromptProperty =
            DependencyProperty.Register("Prompt", typeof(string), typeof(PopupWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(SettingsViewModel), typeof(PopupWindow),
                                        new PropertyMetadata(null));

        public string Prompt
        {
            get { return (string)GetValue(PromptProperty); }
            set { SetValue(PromptProperty, value); }
        }

        public SettingsViewModel ViewModel
        {
            get { return (SettingsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        #endregion

        #region Public Properties

        public DelegateCommand<object> OKCommand { get; set; }

        public DelegateCommand<object> CancelCommand { get; set; }

        public string Text { get; set; }

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _textBox = GetTemplateChild("PART_TextBox") as TextBox;
        }

        #endregion

        #region Private Methods

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
                ViewModel.Callback.Invoke((string)obj);
        }

        private void VisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                Text = string.Empty;

                if (_textBox != null)
                    Dispatcher.BeginInvoke(new Action(() => FocusManager.SetFocusedElement(this, _textBox)), DispatcherPriority.Render);
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}