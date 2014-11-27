namespace Webcal.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Core;
    using DataModel;
    using Library.ViewModels;
    using Views;

    [TemplatePart(Name = "PART_TextBox", Type = typeof (TextBox))]
    public class PopupWindow : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof (SettingsViewModel), typeof (PopupWindow),
                new PropertyMetadata(null));

        public static readonly DependencyProperty PromptProperty =
            DependencyProperty.Register("Prompt", typeof (UserPromptViewModel), typeof (PopupWindow), new PropertyMetadata(null, OnPromptChanged) );

        private TextBox _textBox;

        public PopupWindow()
        {
            OKCommand = new DelegateCommand<object>(OnOK);
            CancelCommand = new DelegateCommand<object>(OnCancel);

            IsVisibleChanged += VisibilityChanged;
        }

        public UserPromptViewModel Prompt
        {
            get { return (UserPromptViewModel) GetValue(PromptProperty); }
            set { SetValue(PromptProperty, value); }
        }



        public bool HasSecondPrompt
        {
            get { return (bool)GetValue(HasSecondPromptProperty); }
            set { SetValue(HasSecondPromptProperty, value); }
        }

        public static readonly DependencyProperty HasSecondPromptProperty =
            DependencyProperty.Register("HasSecondPrompt", typeof(bool), typeof(PopupWindow));

        

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
            {
                ViewModel.IsPromptVisible = false;
            }
        }

        private void OnOK(object obj)
        {
            if (ViewModel == null)
            {
                return;
            }

            ViewModel.IsPromptVisible = false;

            if (ViewModel.Callback != null)
            {
                ViewModel.Callback.Invoke(Prompt);
            }
        }

        private void VisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool) e.NewValue)
            {
                Text = string.Empty;

                if (_textBox != null)
                {
                    Dispatcher.BeginInvoke(new Action(() => FocusManager.SetFocusedElement(this, _textBox)), DispatcherPriority.Render);
                }
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

        private static void OnPromptChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var userPromptViewModel = e.NewValue as UserPromptViewModel;
            if (userPromptViewModel != null)
            {
                var popupWindow = d as PopupWindow;
                if (popupWindow != null)
                {
                    popupWindow.HasSecondPrompt = userPromptViewModel.HasSecondPrompt;
                }
            }
        }
    }
}