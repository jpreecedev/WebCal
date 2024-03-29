﻿namespace TachographReader.Windows
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Connect.Shared;
    using Connect.Shared.Models;
    using Core;
    using DataModel.Core;
    using DataModel.Library;
    using Library;
    using Library.PDF;
    using Library.ViewModels;
    using Properties;
    using Shared;
    using Views;
    using Views.Settings;

    public class MainWindowViewModel : BaseNavigationViewModel
    {
        public MainWindowViewModel()
        {
            ShowView<HomeScreenView>();
        }

        public DelegateCommand<object> SettingsCommand { get; set; }
        public DelegateCommand<object> GenerateReportCommand { get; set; }
        public DelegateCommand<object> M1N1LabelCommand { get; set; }
        public DelegateCommand<object> TechniciansCommand { get; set; }
        public DelegateCommand<object> BackupDatabaseCommand { get; set; }
        public DelegateCommand<object> RestoreDatabaseCommand { get; set; }
        public DelegateCommand<object> CopyWorkshopCardCommand { get; set; }
        public DelegateCommand<Window> ExitCommand { get; set; }
        public DelegateCommand<object> HomeScreenCommand { get; set; }
        public DelegateCommand<object> NewTachographCommand { get; set; }
        public DelegateCommand<object> NewAnalogueTachographCommand { get; set; }
        public DelegateCommand<object> NewUndownloadabilityCommand { get; set; }
        public DelegateCommand<object> DocumentHistoryCommand { get; set; }
        public DelegateCommand<object> TachographHistoryCommand { get; set; }
        public DelegateCommand<object> UndownloadabilityHistoryCommand { get; set; }
        public DelegateCommand<object> CalibrationsCommand { get; set; }
        public DelegateCommand<object> DriverCardFilesCommand { get; set; }
        public DelegateCommand<object> WorkshopCardFilesCommand { get; set; }
        public DelegateCommand<object> ShowWindowCommand { get; set; }
        public DelegateCommand<object> ExitTrayCommand { get; set; }
        public DelegateCommand<object> CancelCommand { get; set; }
        public DelegateCommand<object> NewSpeedlimiterCommand { get; set; }
        public DelegateCommand<object> ViewTechnicalInformationCommand { get; set; }
        public DelegateCommand<object> ViewTaskQueueCommand { get; set; }
        public DelegateCommand<object> UpdateCommand { get; set; }
        public DelegateCommand<UserControl> CloseModalCommand { get; set; }
        public DelegateCommand<UserControl> SaveModalCommand { get; set; }
        public DelegateCommand<object> LetterForDecommissioningCommand { get; set; }
        public DelegateCommand<object> LetterForDecommissioningHistoryCommand { get; set; }
        public DelegateCommand<object> QCCheckHistoryCommand { get; set; }
        public DelegateCommand<object> QC3MonthCheckHistoryCommand { get; set; }
        public DelegateCommand<object> QCCheckCommand { get; set; }
        public DelegateCommand<object> QC6MonthCheckCommand { get; set; }

        public DispatcherTimer TimeoutTimer { get; set; }
        public bool IsLocked { get; set; }
        public bool ShowConnectProgress { get; set; }

        private void OnSettings(object param)
        {
            ShowSettingsModalView();
        }

        private void OnGenerateReport(object param)
        {
            ShowMediumModal<GenerateReportView>();
        }

        private void OnM1N1(object param)
        {
            ShowXLargeModal<M1N1View>();
        }

        private void OnTechnicians(object param)
        {
            ShowLargeModal<TechniciansView>();
        }

        private void OnBackupDatabase(object param)
        {
            BackupRestoreManager.BackUp();
        }

        private void OnRestoreDatabase(object param)
        {
            BackupRestoreManager.Restore();
        }

        private void OnCopyWorkshopCard(object param)
        {
        }

        private void OnExit(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }

        private void OnHomeScreenCommand(object param)
        {
            ShowView<HomeScreenView>();
        }

        private void OnNewTachograph(object param)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }

            var viewModel = (NewTachographViewModel)ShowView<NewTachographView>();
            viewModel.SetDocumentTypes(true);
        }

        private void OnNewAnalogueTachograph(object param)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }

            var viewModel = (NewTachographViewModel)ShowView<NewAnalogueTachographView>();
            viewModel.SetDocumentTypes(false);
        }

        private void OnNewUndownloadability(object param)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }

            ShowView<NewUndownloadabilityView>();
        }

        private void OnTachographHistory(object param)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }

            ShowView<DocumentHistoryView>().Loaded = viewModel =>
            {
                ((DocumentHistoryViewModel)viewModel).SelectedDocumentType = typeof(TachographDocument).Name.SplitByCapitals();
            };
        }

        private void OnUndownloadabilityHistory(object param)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }

            ShowView<DocumentHistoryView>().Loaded = viewModel =>
            {
                ((DocumentHistoryViewModel)viewModel).SelectedDocumentType = typeof(UndownloadabilityDocument).Name.SplitByCapitals();
            };
        }

        private void OnLetterForDecommissioningHistory(object param)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }

            ShowView<DocumentHistoryView>().Loaded = viewModel =>
            {
                ((DocumentHistoryViewModel)viewModel).SelectedDocumentType = typeof(LetterForDecommissioningDocument).Name.SplitByCapitals();
            };
        }

        private void OnQCCheckHistory(object obj)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }

            ShowView<DocumentHistoryView>().Loaded = viewModel =>
            {
                ((DocumentHistoryViewModel)viewModel).SelectedDocumentType = typeof(QCReport).Name.SplitByCapitals();
            };
        }

        private void On3MonthCheckHistoryCommand(object obj)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }

            ShowView<DocumentHistoryView>().Loaded = viewModel =>
            {
                ((DocumentHistoryViewModel)viewModel).SelectedDocumentType = "QC 3 Month Walkaround";
            };
        }

        private void OnDocumentHistory(object param)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }

            ShowView<DocumentHistoryView>().Loaded = viewModel =>
            {
                ((DocumentHistoryViewModel)viewModel).SelectedDocumentType = Resources.TXT_SELECT_ALL;
            };
        }

        private void OnCalibrations(object param)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }

            ShowView<CalibrationsView>();
        }

        private void OnDriverCardFiles(object param)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }

            ShowView<DriverCardFilesView>();
        }

        private void OnWorkshopCardFiles(object param)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }

            ShowView<WorkshopCardFilesView>();
        }

        private void OnLetterForDecommissioning(object param)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }

            ShowView<LetterForDecommissioningView>();
        }

        private void OnQCCheck(object param)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }

            ShowView<QCCheckView>();
        }

        private void OnQC6MonthCheck(object obj)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }

            ShowView<QC6MonthCheckView>();
        }

        private static void OnShowWindow(object obj)
        {
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }

        private static void OnExitTray(object obj)
        {
            Application.Current.MainWindow.Close();
        }

        private void OnCancel(object obj)
        {
            if (HasChanged)
            {
                if (AskQuestion(Resources.TXT_UNSAVED_CHANGES_WILL_BE_LOST))
                {
                    ShowView<HomeScreenView>();
                    return;
                }
            }

            ShowView<HomeScreenView>();
        }

        private void OnNewSpeedlimiter(object obj)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }

            ShowView<SpeedlimiterView>();
        }

        private static void OnViewTechnicalInformation(object obj)
        {
            var exceptionWindow = new ExceptionWindow.ExceptionWindow();
            exceptionWindow.ShowDialog();
        }

        private static void OnViewTaskQueue(object obj)
        {
            var workerProgressWindow = new WorkerProgressWindow.WorkerProgressWindow();
            workerProgressWindow.ShowDialog();
        }

        private void OnUpdate(object arg)
        {
            Process.Start("AutoUpdater.exe", "/checknow");
        }

        private void OnSaveModal(UserControl view)
        {
            if (!IsValid(view))
            {
                ShowError(Resources.EXC_MISSING_FIELDS);
                return;
            }

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                CloseSettingsModal(true);

                var documentViewModel = View.DataContext as INewDocumentViewModel;
                if (documentViewModel != null)
                {
                    documentViewModel.OnModalClosed();
                }
            }
            catch (Exception ex)
            {
                ShowError(Resources.ERR_SAVING_SETTINGS, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex));
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void OnCloseModal(UserControl obj)
        {
            IsModalWindowVisible = false;

            var dataContext = ModalView.DataContext as BaseNavigationViewModel;
            if (dataContext != null)
            {
                dataContext.Dispose();
            }

            ModalView.DataContext = null;
            ModalView = null;
        }

        public IViewModel ShowView<T>() where T : UserControl, new()
        {
            return ShowView<T>(this);
        }

        protected override void InitialiseCommands()
        {
            SettingsCommand = new DelegateCommand<object>(OnSettings);
            GenerateReportCommand = new DelegateCommand<object>(OnGenerateReport);
            M1N1LabelCommand = new DelegateCommand<object>(OnM1N1);
            TechniciansCommand = new DelegateCommand<object>(OnTechnicians);
            BackupDatabaseCommand = new DelegateCommand<object>(OnBackupDatabase);
            RestoreDatabaseCommand = new DelegateCommand<object>(OnRestoreDatabase);
            CopyWorkshopCardCommand = new DelegateCommand<object>(OnCopyWorkshopCard);
            ExitCommand = new DelegateCommand<Window>(OnExit);
            HomeScreenCommand = new DelegateCommand<object>(OnHomeScreenCommand);
            NewTachographCommand = new DelegateCommand<object>(OnNewTachograph);
            NewAnalogueTachographCommand = new DelegateCommand<object>(OnNewAnalogueTachograph);
            NewUndownloadabilityCommand = new DelegateCommand<object>(OnNewUndownloadability);
            TachographHistoryCommand = new DelegateCommand<object>(OnTachographHistory);
            UndownloadabilityHistoryCommand = new DelegateCommand<object>(OnUndownloadabilityHistory);
            DocumentHistoryCommand = new DelegateCommand<object>(OnDocumentHistory);
            CalibrationsCommand = new DelegateCommand<object>(OnCalibrations);
            DriverCardFilesCommand = new DelegateCommand<object>(OnDriverCardFiles);
            WorkshopCardFilesCommand = new DelegateCommand<object>(OnWorkshopCardFiles);
            ShowWindowCommand = new DelegateCommand<object>(OnShowWindow);
            ExitTrayCommand = new DelegateCommand<object>(OnExitTray);
            NewSpeedlimiterCommand = new DelegateCommand<object>(OnNewSpeedlimiter);
            CancelCommand = new DelegateCommand<object>(OnCancel);
            ViewTechnicalInformationCommand = new DelegateCommand<object>(OnViewTechnicalInformation);
            ViewTaskQueueCommand = new DelegateCommand<object>(OnViewTaskQueue);
            UpdateCommand = new DelegateCommand<object>(OnUpdate);
            SaveModalCommand = new DelegateCommand<UserControl>(OnSaveModal);
            CloseModalCommand = new DelegateCommand<UserControl>(OnCloseModal);
            LetterForDecommissioningCommand = new DelegateCommand<object>(OnLetterForDecommissioning);
            LetterForDecommissioningHistoryCommand = new DelegateCommand<object>(OnLetterForDecommissioningHistory);
            QCCheckHistoryCommand = new DelegateCommand<object>(OnQCCheckHistory);
            QC3MonthCheckHistoryCommand = new DelegateCommand<object>(On3MonthCheckHistoryCommand);
            QCCheckCommand = new DelegateCommand<object>(OnQCCheck);
            QC6MonthCheckCommand = new DelegateCommand<object>(OnQC6MonthCheck);
        }

        protected override void Load()
        {
            base.Load();

            ConnectHelper.SyncData();
            
            if (!HasValidLicense())
            {
                return;
            }

            var settingsRepository = GetInstance<ISettingsRepository<WorkshopSettings>>();
            var workshopSettings = settingsRepository.GetWorkshopSettings();
            if (workshopSettings.IsStatusReportCheckEnabled)
            {
                var allTechnicians = GetInstance<IRepository<Technician>>().Where(c => c.Deleted == null).ToList();
                var statusReport = new StatusReportViewModel(allTechnicians);
                if (!statusReport.IsUpToDate())
                {
                    if (ShowWarning(Resources.TXT_INFO_OUT_OF_DATE, Resources.TXT_OUT_OF_DATE_TITLE, MessageBoxButton.YesNo))
                    {
                        statusReport.GenerateStatusReport();
                        workshopSettings.CentreQuarterlyCheckDate = DateTime.Now.Date;
                        settingsRepository.Save(workshopSettings);
                    }
                }
            }
        }

        private void CloseSettingsModal(bool save)
        {
            var settingsView = ModalView as SettingsView;
            if (settingsView != null)
            {
                var vm = (SettingsViewModel)settingsView.DataContext;

                try
                {
                    vm.ClearSettingsViewCache(save);
                    vm.OnClosing(!save);
                    vm.Dispose();
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex);
                }
            }
            IsModalWindowVisible = false;
        }
    }
}