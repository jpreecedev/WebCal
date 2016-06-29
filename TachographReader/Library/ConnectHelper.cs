namespace TachographReader.Library
{
    using System;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Windows;
    using Windows;
    using Connect.Shared;
    using Connect.Shared.Models;
    using DataModel;
    using DataModel.Core;
    using Shared;
    using Shared.Connect;
    using Windows.ProgressWindow;
    using DataModel.Library;
    using DataModel.Properties;
    using PDF;
    using ViewModels;

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

        public static void SyncData()
        {
            CallAsync(() =>
            {
                SyncWorkshopSettings();
                SyncTechnicians();
                SyncCustomerContacts();
                SyncDocuments();
                SyncExceptions();
            });
        }

        public static void Upload(TachographDocument document, bool isUpdating = false)
        {
            CallAsync(() =>
            {
                if (isUpdating)
                {
                    _connectClient.Service.UpdateTachographDocument(CheckSerializedData(document));
                }
                else
                {
                    _connectClient.Service.UploadTachographDocument(CheckSerializedData(document));
                }
                SaveDocumentUpload(document);
            });
        }

        public static void Upload(UndownloadabilityDocument document, bool isUpdating = false)
        {
            CallAsync(() =>
            {
                if (isUpdating)
                {
                    _connectClient.Service.UpdateUndownloadabilityDocument(CheckSerializedData(document));
                }
                else
                {
                    _connectClient.Service.UploadUndownloadabilityDocument(CheckSerializedData(document));
                }
                SaveDocumentUpload(document);
            });
        }

        public static void Upload(LetterForDecommissioningDocument document, bool isUpdating = false)
        {
            CallAsync(() =>
            {
                if (isUpdating)
                {
                    _connectClient.Service.UpdateLetterForDecommissioningDocument(CheckSerializedData(document));
                }
                else
                {
                    _connectClient.Service.UploadLetterForDecommissioningDocument(CheckSerializedData(document));
                }
                SaveDocumentUpload(document);
            });
        }

        public static void Upload(QCReport report, bool isUpdating = false)
        {
            CallAsync(() =>
            {
                _connectClient.Service.UploadQCReport(CheckReportSerializedData(report));
                SaveReportUpload(report);
            });
        }

        public static void Upload(QCReport6Month report, bool isUpdating = false)
        {
            CallAsync(() =>
            {
                _connectClient.Service.UploadQCReport6Month(CheckReportSerializedData(report));
                SaveReportUpload(report);
            });
        }

        public static void BackupDatabaseAsync(string databasePath)
        {
            CallAsync(() =>
            {
                var serviceCredentials = _connectClient.Service.GetServiceCredentials();
                FtpHelper.SaveDatabaseBackup(serviceCredentials, databasePath);
            });
        }

        public static void BackupDatabase(string databasePath)
        {
            CallSync(() =>
            {
                var serviceCredentials = _connectClient.Service.GetServiceCredentials();
                FtpHelper.SaveDatabaseBackup(serviceCredentials, databasePath);
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

        private static void SaveCustomerContactUpload(CustomerContact customerContact)
        {
            customerContact.Uploaded = DateTime.Now;

            var repository = ContainerBootstrapper.Resolve<IRepository<CustomerContact>>();
            repository.AddOrUpdate(customerContact);
        }

        private static void SaveTechnicianUpload(Technician technician)
        {
            technician.Uploaded = DateTime.Now;

            var repository = ContainerBootstrapper.Resolve<IRepository<Technician>>();
            repository.AddOrUpdate(technician);
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

        private static T CheckReportSerializedData<T>(T report) where T : BaseReport
        {
            if (report.SerializedData == null)
            {
                report.ToReportPDF();
            }
            return report;
        }

        private static void SyncExceptions()
        {
            var detailedExceptions = ContainerBootstrapper.Resolve<IRepository<DetailedException>>();
            foreach (var detailedException in detailedExceptions.Where(c => c.Uploaded == null))
            {
                _connectClient.Service.UploadDetailedException(detailedException);

                detailedException.Uploaded = DateTime.Now;
                detailedExceptions.AddOrUpdate(detailedException);
            }
        }

        private static void SyncWorkshopSettings()
        {
            var generalSettingsRepository = ContainerBootstrapper.Resolve<ISettingsRepository<WorkshopSettings>>();
            var workshopSettings = generalSettingsRepository.GetWorkshopSettings();
            if (workshopSettings.Uploaded != null)
            {
                return;
            }

            using (var context = new TachographContext())
            {
                context.Configuration.ProxyCreationEnabled = false;

                var settings = context.WorkshopSettings.AsNoTracking()
                    .Where(w => !string.IsNullOrEmpty(w.Address1) ||
                                !string.IsNullOrEmpty(w.Address2) ||
                                !string.IsNullOrEmpty(w.Address3) ||
                                !string.IsNullOrEmpty(w.Office) ||
                                !string.IsNullOrEmpty(w.PostCode) ||
                                !string.IsNullOrEmpty(w.Town) ||
                                !string.IsNullOrEmpty(w.WorkshopName))
                    .First(c => c.Id == workshopSettings.Id);
                
                    _connectClient.Service.UploadWorkshopSettings(settings);
            }

            if (workshopSettings.Created == default(DateTime) || workshopSettings.Created == DateTime.Parse("01/01/1900"))
            {
                workshopSettings.Created = DateTime.Now;
            }
            workshopSettings.Uploaded = DateTime.Now;
            generalSettingsRepository.Save(workshopSettings);
        }

        private static void SyncTechnicians()
        {
            var techniciansRepository = ContainerBootstrapper.Resolve<IRepository<Technician>>();

            foreach (var technician in techniciansRepository.Where(c => c.Uploaded == null))
            {
                _connectClient.Service.UploadTechnician(technician);
                SaveTechnicianUpload(technician);
            }
        }

        private static void SyncCustomerContacts()
        {
            var customerContactsRepository = ContainerBootstrapper.Resolve<IRepository<CustomerContact>>();

            foreach (var customerContact in customerContactsRepository.Where(c => c.Uploaded == null))
            {
                _connectClient.Service.UploadCustomerContact(customerContact);
                SaveCustomerContactUpload(customerContact);
            }
        }

        private static void SyncDocuments()
        {
            var tachographDocumentsRepository = ContainerBootstrapper.Resolve<IRepository<TachographDocument>>();
            var undownloadabilityDocumentsRepository = ContainerBootstrapper.Resolve<IRepository<UndownloadabilityDocument>>();
            var letterForDecommissioningRepository = ContainerBootstrapper.Resolve<IRepository<LetterForDecommissioningDocument>>();
            var qcReportRepository = ContainerBootstrapper.Resolve<IRepository<QCReport>>();
            var qcReport6MonthRepository = ContainerBootstrapper.Resolve<IRepository<QCReport6Month>>();

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
                var report = new QCReportViewModel(qcReport).ToReportPDF();
                qcReport.SerializedData = report.SerializedData;

                var repository = ContainerBootstrapper.Resolve<IRepository<QCReport>>();
                repository.AddOrUpdate(qcReport);
            }
            foreach (var qcReport in qcReportRepository.Where(c => c.Uploaded == null))
            {
                _connectClient.Service.AutoUploadQCReport(CheckReportSerializedData(qcReport));
                SaveReportUpload(qcReport);
            }
            foreach (var qcReport6Month in qcReport6MonthRepository.Where(c => c.Uploaded == null))
            {
                _connectClient.Service.AutoUploadQCReport6Month(CheckReportSerializedData(qcReport6Month));
                SaveReportUpload(qcReport6Month);
            }
        }
    }
}