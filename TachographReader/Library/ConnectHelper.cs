namespace TachographReader.Library
{
    using System;
    using System.Runtime.Caching;
    using System.Windows;
    using Windows;
    using Connect.Shared;
    using Connect.Shared.Models;
    using DataModel;
    using DataModel.Core;
    using Shared;
    using Shared.Connect;
    using System.IO;
    using Windows.ProgressWindow;
    using DataModel.Properties;

    public static class ConnectHelper
    {
        private static readonly IConnectClient _connectClient;

        private static readonly ObjectCache _cache = new MemoryCache("ConnectHelperCache");
        private const string ConnectCacheKey = "ConnectKeysCache";

        private static ProgressWindow _progressWindow;

        static ConnectHelper()
        {
            _connectClient = ContainerBootstrapper.Resolve<IConnectClient>();
        }

        public static void ConnectKeysChanged()
        {
            _cache.Remove(ConnectCacheKey);
        }

        public static bool IsConnectEnabled()
        {
            var registrationData = ContainerBootstrapper.Resolve<IRepository<RegistrationData>>().First();
            return registrationData.IsConnectEnabled && GetConnectKeys() != null;
        }

        public static IConnectKeys GetConnectKeys()
        {
            var cachedKeys = (IConnectKeys) _cache.Get(ConnectCacheKey);
            if (cachedKeys == null)
            {
                var registrationData = ContainerBootstrapper.Resolve<IRepository<RegistrationData>>().First();
                if (registrationData.IsConnectEnabled)
                {
                    cachedKeys = registrationData.ConnectKeys;

                    _cache.Add(new CacheItem(ConnectCacheKey, cachedKeys), new CacheItemPolicy {SlidingExpiration = new TimeSpan(0, 30, 0)});
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

        public static void BackupDatabase(string databasePath)
        {
            CallSync(() =>
            {
                var serviceCredentials = _connectClient.Service.GetServiceCredentials();
                FtpHelper.SaveDatabaseBackup(serviceCredentials, File.ReadAllBytes(databasePath));
            });
        }

        private static void CallAsync(Action action)
        {
            if (!IsConnectEnabled())
            {
                return;
            }

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

        private static void CallSync(Action action)
        {
            if (!IsConnectEnabled())
            {
                return;
            }
            
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
                CloseProgressWindow();
            });

            ShowProgressWindow();
        }

        private static void ToggleConnectProgress(bool show)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            var mainWindowViewModel = mainWindow?.DataContext as MainWindowViewModel;
            if (mainWindowViewModel != null)
            {
                mainWindowViewModel.ShowConnectProgress = show;
            }
        }

        private static void ShowProgressWindow()
        {
            _progressWindow = new ProgressWindow();
            ((ProgressWindowViewModel)_progressWindow.DataContext).ProgressText = Resources.TXT_PROCESSING;
            _progressWindow.ShowDialog();
        }

        private static void CloseProgressWindow()
        {
            if (_progressWindow != null)
            {
                _progressWindow.Close();
                _progressWindow = null;
            }
        }
    }
}