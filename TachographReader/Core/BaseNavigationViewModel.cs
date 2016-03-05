namespace TachographReader.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using Windows;
    using DataModel.Core;
    using EventArguments;
    using Properties;
    using Shared;
    using Views;

    public class BaseNavigationViewModel : BaseViewModel
    {
        private SettingsView _settingsView;
        public EventHandler<ModalClosedEventArgs> ModalClosedEvent;
        private readonly Action<bool, object> _doneCallback;
        private readonly IDictionary<Type, UserControl> _settingsViewCache;

        public BaseNavigationViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;
            }

            _settingsViewCache = new Dictionary<Type, UserControl>();
            _doneCallback = OnSmallWindowClosing;
        }

        public UserControl View { get; set; }
        public UserControl ModalView { get; set; }
        public bool IsModalWindowVisible { get; set; }
        public bool IsSmallModal { get; set; }
        public bool IsMediumModal { get; set; }
        public bool IsLargeModal { get; set; }

        public UserControl ShowSettingsView(Type view)
        {
            IsSmallModal = false;
            if (View != null)
            {
                var viewModel = View.DataContext as IViewModel;
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

            var dataContext = ModalView.DataContext as IViewModel;
            if (dataContext != null)
            {
                dataContext.DoneCallback = _doneCallback;
            }
        }

        public void ShowMediumModal<T>() where T : UserControl, new()
        {
            IsMediumModal = true;
            IsModalWindowVisible = true;
            ModalView = new T();

            var dataContext = ModalView.DataContext as IViewModel;
            if (dataContext != null)
            {
                dataContext.DoneCallback = _doneCallback;
            }
        }

        public void ShowLargeModal<T>() where T : UserControl, new()
        {
            IsMediumModal = false;
            IsLargeModal = true;
            IsModalWindowVisible = true;
            ModalView = new T();

            var dataContext = ModalView.DataContext as IViewModel;
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
                var viewModel = (IViewModel) View.DataContext;
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
            {
                return;
            }

            foreach (var settingsView in _settingsViewCache)
            {
                UserControl view = settingsView.Value;
                if (view == null)
                {
                    continue;
                }

                var viewModel = view.DataContext as BaseSettingsViewModel;
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

        protected void ShowModalView<T>() where T : UserControl, new()
        {
            IsSmallModal = false;
            IsMediumModal = false;
            IsLargeModal = false;
            IsModalWindowVisible = true;
            ModalView = new T();
        }

        protected void ShowSettingsModalView()
        {
            IsSmallModal = false;
            IsMediumModal = false;
            IsLargeModal = false;
            IsModalWindowVisible = true;

            ModalView = _settingsView ?? (_settingsView = new SettingsView());
            ModalView.DataContext = new SettingsViewModel();
        }

        private IViewModel CreateView<T>(MainWindowViewModel mainWindowViewModel, IViewModel viewModel) where T : UserControl, new()
        {
            if (viewModel != null)
            {
                viewModel.OnClosing(false);
                viewModel.Dispose();
            }

            View = new T();

            var dataContext = View.DataContext as IViewModel;
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
            IsMediumModal = false;
            IsLargeModal = false;

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
                ShowError(Resources.EXC_UNABLE_CREATE_SETTINGS_VIEW, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex));
            }

            return null;
        }
    }
}