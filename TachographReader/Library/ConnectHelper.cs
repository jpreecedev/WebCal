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
    using PDF;

    public static class ConnectHelper
    {
        private static readonly IConnectClient _connectClient;

        private static readonly ObjectCache _cache = new MemoryCache("ConnectHelperCache");
        private const string CONNECT_CACHE_KEY = "ConnectKeysCache";

        private static ProgressWindow _progressWindow;

        static ConnectHelper()
        {
            _connectClient = ContainerBootstrapper.Resolve<IConnectClient>();
        }

        public static void ConnectKeysChanged()
        {
            _cache.Remove(CONNECT_CACHE_KEY);
        }

        public static bool IsConnectEnabled()
        {
            var registrationData = ContainerBootstrapper.Resolve<IRepository<RegistrationData>>().First();
            return registrationData.IsConnectEnabled && GetConnectKeys() != null;
        }

        public static IConnectKeys GetConnectKeys()
        {
            var cachedKeys = (IConnectKeys)_cache.Get(CONNECT_CACHE_KEY);
            if (cachedKeys == null)
            {
                var registrationData = ContainerBootstrapper.Resolve<IRepository<RegistrationData>>().First();
                if (registrationData.IsConnectEnabled)
                {
                    cachedKeys = registrationData.ConnectKeys;

                    _cache.Add(new CacheItem(CONNECT_CACHE_KEY, cachedKeys), new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 30, 0) });
                }
            }
            if (cachedKeys == null || cachedKeys.LicenseKey == 0)
            {
                return null;
            }
            return cachedKeys;
        }

        public static void SyncDocuments()
        {
            var tachographDocumentsRepository = ContainerBootstrapper.Resolve<IRepository<TachographDocument>>();
            var undownloadabilityDocumentsRepository = ContainerBootstrapper.Resolve<IRepository<UndownloadabilityDocument>>();
            var letterForDecommissioningRepository = ContainerBootstrapper.Resolve<IRepository<LetterForDecommissioningDocument>>();
            var qcReportRepository = ContainerBootstrapper.Resolve<IRepository<QCReport>>();
            var qcReport3MonthRepository = ContainerBootstrapper.Resolve<IRepository<QCReport3Month>>();

            CallAsync(() =>
            {
                foreach (var tachographDocument in tachographDocumentsRepository.Where(c => c.Uploaded == null))
                {
                    _connectClient.Service.AutoUploadTachographDocument(CheckSerializedData(tachographDocument));
                    SaveDocumentUpload(tachographDocument);
                }
                foreach (var undownloadabilityDocument in undownloadabilityDocumentsRepository.Where(c => c.Uploaded == null))
                {
                    _connectClient.Service.AutoUploadUndownloadabilityDocument(CheckSerializedData(undownloadabilityDocument));
                    SaveDocumentUpload(undownloadabilityDocument);
                }
                foreach (var letterForDecommissioningDocument in letterForDecommissioningRepository.Where(c => c.Uploaded == null))
                {
                    _connectClient.Service.AutoUploadLetterForDecommissioningDocument(CheckSerializedData(letterForDecommissioningDocument));
                    SaveDocumentUpload(letterForDecommissioningDocument);
                }
                foreach (var qcReport in qcReportRepository.Where(c => c.Uploaded == null))
                {
                    _connectClient.Service.AutoUploadQCReport(qcReport);
                    SaveReportUpload(qcReport);
                }
                foreach (var qcReport3Month in qcReport3MonthRepository.Where(c => c.Uploaded == null))
                {
                    _connectClient.Service.AutoUploadQCReport3Month(qcReport3Month);
                    SaveReportUpload(qcReport3Month);
                }
            });
        }

        public static void Upload(TachographDocument document)
        {
            CallAsync(() =>
            {
                _connectClient.Service.UploadTachographDocument(document);
                SaveDocumentUpload(document);
            });
        }

        public static void Upload(UndownloadabilityDocument document)
        {
            CallAsync(() =>
            {
                _connectClient.Service.UploadUndownloadabilityDocument(document);
                SaveDocumentUpload(document);
            });
        }

        public static void Upload(LetterForDecommissioningDocument document)
        {
            CallAsync(() =>
            {
                _connectClient.Service.UploadLetterForDecommissioningDocument(document);
                SaveDocumentUpload(document);
            });
        }

        public static void Upload(QCReport report)
        {
            CallAsync(() =>
            {
                _connectClient.Service.UploadQCReport(report);
                SaveReportUpload(report);
            });
        }

        public static void Upload(QCReport3Month report)
        {
            CallAsync(() =>
            {
                _connectClient.Service.UploadQCReport3Month(report);
                SaveReportUpload(report);
            });
        }

        public static void BackupDatabaseAsync(string databasePath)
        {
            CallAsync(() =>
            {
                var serviceCredentials = _connectClient.Service.GetServiceCredentials();
                FtpHelper.SaveDatabaseBackup(serviceCredentials, File.ReadAllBytes(databasePath));
            });
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

        private static void SaveDocumentUpload<T>(T document) where T : Document
        {
            document.Uploaded = DateTime.Now;

            var repository = ContainerBootstrapper.Resolve<IRepository<T>>();
            repository.AddOrUpdate(document);
        }

        private static void SaveReportUpload<T>(T report) where T : BaseReport
        {
            report.Uploaded = DateTime.Now;

            var repository = ContainerBootstrapper.Resolve<IRepository<T>>();
            repository.AddOrUpdate(report);
        }

        private static T CheckSerializedData<T>(T document) where T : Document
        {
            if (document.SerializedData == null)
            {
                document.ToPDF();
            }
            return document;
        }
    }
}