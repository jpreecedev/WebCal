namespace Webcal.Library
{
    using System;
    using System.Runtime.Caching;
    using System.Threading;
    using System.Windows;
    using Windows;
    using Connect.Shared;
    using Connect.Shared.Models;
    using DataModel;
    using DataModel.Core;
    using Shared;
    using Shared.Connect;

    public static class ConnectHelper
    {
        private static readonly IConnectClient _connectClient;

        private static readonly ObjectCache _cache = new MemoryCache("ConnectHelperCache");
        private const string ConnectCacheKey = "ConnectKeysCache";

        static ConnectHelper()
        {
            _connectClient = ContainerBootstrapper.Container.GetInstance<IConnectClient>();
        }

        public static void ConnectKeysChanged()
        {
            _cache.Remove(ConnectCacheKey);
        }

        public static IConnectKeys GetConnectKeys()
        {
            var cachedKeys = (IConnectKeys)_cache.Get(ConnectCacheKey);
            if (cachedKeys == null)
            {
                var registrationData = ContainerBootstrapper.Container.GetInstance<IRepository<RegistrationData>>().First();
                if (registrationData.IsConnectEnabled)
                {
                    cachedKeys = registrationData.ConnectKeys;

                    _cache.Add(new CacheItem(ConnectCacheKey, cachedKeys), new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 30, 0) });
                }
            }
            if (cachedKeys == null || cachedKeys.LicenseKey == 0)
            {
                return null;
            }
            return cachedKeys;
        }

        public static void Upload(TachographDocument document)
        {
            CallAsync(() => _connectClient.Service.UploadTachographDocument(document));
        }

        public static void Upload(UndownloadabilityDocument document)
        {
            CallAsync(() => _connectClient.Service.UploadUndownloadabilityDocument(document));
        }

        public static void Upload(LetterForDecommissioningDocument document)
        {
            CallAsync(() => _connectClient.Service.UploadLetterForDecommissioningDocument(document));
        }

        private static void CallAsync(Action action)
        {
            ToggleConnectProgress(true);

            _connectClient.CallAsync(GetConnectKeys(), client =>
            {
                action.Invoke();
            },
            exceptionHandler: exception =>
            {
                ExceptionPolicy.HandleException(ContainerBootstrapper.Container, exception);
            },
            alwaysCall: () =>
            {
                ToggleConnectProgress(false);
            });
        }

        private static void ToggleConnectProgress(bool show)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                var mainWindowViewModel = mainWindow.DataContext as MainWindowViewModel;
                if (mainWindowViewModel != null)
                {
                    mainWindowViewModel.ShowConnectProgress = show;
                }
            }
        }
    }
}