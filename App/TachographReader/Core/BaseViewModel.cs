﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Webcal.Library;
using Webcal.Views;
using Webcal.Windows;

namespace Webcal.Core
{
    public class BaseViewModel : BaseNotification, IViewModel
    {
        #region Constructor

        public BaseViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(InternalLoad), DispatcherPriority.Loaded);
        }

        #endregion

        #region Public Properties

        public bool HasChanged { get; protected set; }

        public MainWindowViewModel MainWindow { get; set; }

        public Action<bool, object> DoneCallback { get; set; }

        #endregion

        #region Public Methods

        public virtual void OnClosing(bool cancelled)
        {

        }

        #endregion

        #region Protected Methods

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

        protected void GetInputFromUser(UserControl window, string prompt, Action<string> callback)
        {
            if (window == null || string.IsNullOrEmpty(prompt) || callback == null)
                return;

            SettingsViewModel settingsViewModel = window.DataContext as SettingsViewModel;
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
                DoneCallback.Invoke(true, parameter);
        }

        protected void Cancelled(object parameter)
        {
            if (DoneCallback != null)
                DoneCallback.Invoke(false, parameter);
        }

        #endregion

        #region Private Methods

        private void InternalLoad()
        {
            InitialiseCommands();
            BeforeLoad();
            InitialiseRepositories();
            Load();
            AfterLoad();
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {

        }

        #endregion
    }
}