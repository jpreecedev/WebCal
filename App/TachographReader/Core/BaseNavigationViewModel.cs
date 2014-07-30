using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Webcal.EventArguments;
using Webcal.Shared;
using Webcal.Views;
using Webcal.Windows;
using Webcal.Properties;

namespace Webcal.Core
{
    public class BaseNavigationViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly IDictionary<Type, UserControl> _settingsViewCache;
        private SettingsView _settingsView;
        private readonly Action<bool, object> _doneCallback;

        #endregion

        #region Events

        public EventHandler<ModalClosedEventArgs> ModalClosedEvent;

        #endregion

        #region Constructor

        public BaseNavigationViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            _settingsViewCache = new Dictionary<Type, UserControl>();
            _doneCallback = OnSmallWindowClosing;
        }

        #endregion

        #region Public Properties

        public UserControl View { get; set; }

        public UserControl ModalView { get; set; }

        public bool IsModalWindowVisible { get; set; }

        public bool IsSmallModal { get; set; }

        #endregion

        #region Public Methods

        public UserControl ShowSettingsView(Type view)
        {
            IsSmallModal = false;
            if (View != null)
            {
                IViewModel viewModel = View.DataContext as IViewModel;
                if (viewModel != null && viewModel.HasChanged)
                {
                    if (AskQuestion(Resources.TXT_UNSAVED_CHANGES_WILL_BE_LOST))
                    {
                        return View = GetSettingsView(view);
                    }
                }
            }

            return View = GetSettingsView(view);
        }

        public void ShowSmallModal<T>() where T : UserControl, new()
        {
            IsSmallModal = true;
            IsModalWindowVisible = true;
            ModalView = new T();

            IViewModel dataContext = ModalView.DataContext as IViewModel;
            if (dataContext != null)
            {
                dataContext.DoneCallback = _doneCallback;
            }
        }

        protected IViewModel ShowView<T>(MainWindowViewModel mainWindowViewModel) where T : UserControl, new()
        {
            IsSmallModal = false;

            if (View != null)
            {
                IViewModel viewModel = (IViewModel)View.DataContext;
                if (viewModel != null && viewModel.HasChanged)
                {
                    if (AskQuestion(Resources.TXT_UNSAVED_CHANGES_WILL_BE_LOST))
                    {
                        return CreateView<T>(mainWindowViewModel, viewModel);
                    }
                }
                else
                {
                    return CreateView<T>(mainWindowViewModel, viewModel);
                }
            }

            return CreateView<T>(mainWindowViewModel, null);
        }

        public void ClearSettingsViewCache(bool save)
        {
            if (_settingsViewCache == null)
                return;

            foreach (KeyValuePair<Type, UserControl> settingsView in _settingsViewCache)
            {
                UserControl view = settingsView.Value;
                if (view == null) continue;

                BaseSettingsViewModel viewModel = view.DataContext as BaseSettingsViewModel;
                if (viewModel != null)
                {
                    if (save)
                    {
                        viewModel.Save();
                    }

                    viewModel.OnClosing(!save);
                    viewModel.Dispose();
                }
            }
        }

        #endregion

        #region Protected Methods

        protected void ShowModalView<T>() where T : UserControl, new()
        {
            IsSmallModal = false;
            IsModalWindowVisible = true;
            ModalView = new T();
        }

        protected void ShowSettingsModalView()
        {
            IsSmallModal = false;
            IsModalWindowVisible = true;

            ModalView = _settingsView ?? (_settingsView = new SettingsView());
            ModalView.DataContext = new SettingsViewModel();
        }

        #endregion

        #region Private Methods

        private IViewModel CreateView<T>(MainWindowViewModel mainWindowViewModel, IViewModel viewModel) where T : UserControl, new()
        {
            if (viewModel != null)
            {
                viewModel.OnClosing(false);
                viewModel.Dispose();
            }

            View = new T();

            IViewModel dataContext = View.DataContext as IViewModel;
            if (dataContext != null)
            {
                dataContext.MainWindow = mainWindowViewModel;
            }

            return View.DataContext as IViewModel;
        }

        private void OnSmallWindowClosing(bool saved, object parameter)
        {
            IsModalWindowVisible = false;
            IsSmallModal = false;

            OnSmallModalClosed(saved, parameter);
        }

        private void OnSmallModalClosed(bool saved, object parameter)
        {
            if (ModalClosedEvent != null)
            {
                ModalClosedEvent.Invoke(this, new ModalClosedEventArgs(saved, parameter));
            }
        }

        private UserControl GetSettingsView(Type type)
        {
            if (_settingsViewCache.ContainsKey(type))
            {
                return _settingsViewCache[type];
            }

            try
            {
                _settingsViewCache.Add(new KeyValuePair<Type, UserControl>(type, Activator.CreateInstance(type) as UserControl));
                return GetSettingsView(type);
            }
            catch (Exception ex)
            {
                ShowError(Resources.EXC_UNABLE_CREATE_SETTINGS_VIEW, ExceptionPolicy.HandleException(ex));
            }

            return null;
        }


        #endregion

    }
}
