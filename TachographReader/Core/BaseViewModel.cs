namespace TachographReader.Core
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using Windows;
    using Connect.Shared.Models;
    using DataModel.Core;
    using Library;
    using Library.ViewModels;
    using Shared.Helpers;
    using Views;

    public class BaseViewModel : BaseNotification, IViewModel
    {
        public BaseViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(InternalLoad), DispatcherPriority.Loaded);
        }

        public bool HasChanged { get; protected set; }
        public MainWindowViewModel MainWindow { get; set; }
        public Action<bool, object> DoneCallback { get; set; }

        public TService GetInstance<TService>()
        {
            return ContainerBootstrapper.Resolve<TService>();
        }

        public virtual void OnClosing(bool cancelled)
        {
        }

        public virtual void Dispose()
        {
        }

        protected bool AskQuestion(string msg)
        {
            return MessageBoxHelper.AskQuestion(msg);
        }

        protected void ShowMessage(string msg, string caption)
        {
            MessageBoxHelper.ShowMessage(msg, caption);
        }

        protected void ShowError(string msg)
        {
            MessageBoxHelper.ShowError(msg);
        }

        protected void ShowError(string msg, params object[] parameters)
        {
            MessageBoxHelper.ShowError(msg, parameters);
        }

        protected void GetInputFromUser(UserControl window, string prompt, Action<UserPromptViewModel> callback)
        {
            GetInputFromUser(window, new UserPromptViewModel {FirstPrompt = prompt}, callback);
        }

        protected void GetInputFromUser(UserControl window, UserPromptViewModel prompt, Action<UserPromptViewModel> callback)
        {
            if (window == null || prompt == null || callback == null)
            {
                return;
            }

            var settingsViewModel = window.DataContext as SettingsViewModel;
            if (settingsViewModel != null)
            {
                settingsViewModel.IsPromptVisible = true;
                settingsViewModel.Prompt = prompt;
                settingsViewModel.Callback = callback;
            }
        }

        protected virtual void InitialiseCommands()
        {
        }

        protected virtual void InitialiseRepositories()
        {
        }

        protected bool IsValid(DependencyObject root)
        {
            bool isValid = true;

            foreach (IValidate child in root.FindValidatableChildren().Where(child => !child.IsValid()))
            {
                isValid = false;
            }

            return isValid;
        }

        protected virtual void BeforeLoad()
        {
        }

        protected virtual void Load()
        {
        }

        protected virtual void AfterLoad()
        {
            HasChanged = false;
        }

        protected void Saved(object parameter)
        {
            if (DoneCallback != null)
            {
                DoneCallback.Invoke(true, parameter);
            }
        }

        protected void Cancelled(object parameter)
        {
            if (DoneCallback != null)
            {
                DoneCallback.Invoke(false, parameter);
            }
        }

        private void InternalLoad()
        {
            InitialiseCommands();
            BeforeLoad();
            InitialiseRepositories();
            Load();
            AfterLoad();
        }
    }
}