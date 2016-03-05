namespace TachographReader.Controls
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Core;
    using Library.ViewModels;
    using Views;

    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    public class PopupWindow : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(SettingsViewModel), typeof(PopupWindow),
                new PropertyMetadata(null));

        public static readonly DependencyProperty PromptProperty =
            DependencyProperty.Register("Prompt", typeof(UserPromptViewModel), typeof(PopupWindow), new PropertyMetadata(null, OnPromptChanged));

        public static readonly DependencyProperty HasSecondPromptProperty =
            DependencyProperty.Register("HasSecondPrompt", typeof(bool), typeof(PopupWindow));


        public static readonly DependencyProperty TechniciansViewModelProperty =
            DependencyProperty.Register("TechniciansViewModel", typeof(TechniciansViewModel), typeof(PopupWindow));

        public static readonly DependencyProperty HasDatePromptProperty =
            DependencyProperty.Register("HasDatePrompt", typeof(bool), typeof(PopupWindow), new PropertyMetadata(false));


        private TextBox _textBox;

        public PopupWindow()
        {
            OKCommand = new DelegateCommand<object>(OnOK);
            CancelCommand = new DelegateCommand<object>(OnCancel);

            IsVisibleChanged += VisibilityChanged;
        }


        public bool HasDatePrompt
        {
            get { return (bool) GetValue(HasDatePromptProperty); }
            set { SetValue(HasDatePromptProperty, value); }
        }

        public UserPromptViewModel Prompt
        {
            get { return (UserPromptViewModel) GetValue(PromptProperty); }
            set { SetValue(PromptProperty, value); }
        }


        public bool HasSecondPrompt
        {
            get { return (bool) GetValue(HasSecondPromptProperty); }
            set { SetValue(HasSecondPromptProperty, value); }
        }


        public SettingsViewModel ViewModel
        {
            get { return (SettingsViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }


        public TechniciansViewModel TechniciansViewModel
        {
            get { return (TechniciansViewModel) GetValue(TechniciansViewModelProperty); }
            set { SetValue(TechniciansViewModelProperty, value); }
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
            if (TechniciansViewModel != null)
            {
                TechniciansViewModel.IsPromptVisible = false;
            }
        }

        private void OnOK(object obj)
        {
            if (ViewModel == null && TechniciansViewModel == null)
            {
                return;
            }

            if (ViewModel != null)
            {
                ViewModel.IsPromptVisible = false;
                if (ViewModel.Callback != null)
                {
                    ViewModel.Callback.Invoke(Prompt);
                }
            }
            if (TechniciansViewModel != null)
            {
                TechniciansViewModel.IsPromptVisible = false;
                if (TechniciansViewModel.Callback != null)
                {
                    TechniciansViewModel.Callback.Invoke(Prompt);
                }
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                    popupWindow.HasDatePrompt = userPromptViewModel.HasDatePrompt;
                }
            }
        }
    }
}