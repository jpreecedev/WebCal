using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Webcal.Core;
using Webcal.DataModel.Library;
using Webcal.Library;
using Webcal.Shared;
using Webcal.Views;
using Webcal.Properties;

namespace Webcal.Windows
{
    public class MainWindowViewModel : BaseNavigationViewModel
    {
        #region Constructor

        public MainWindowViewModel()
        {
            ShowView<HomeScreenView>();
            //MR commented out the below as per request from Andy
            //short term update, this will be put back at some point
            //InitialiseTimeoutTimer();
        }

        private void InitialiseTimeoutTimer()
        {
            TimeoutTimer = new DispatcherTimer();
            TimeoutTimer.Tick += OnTimeoutTimerTick;
            TimeoutTimer.Interval = new TimeSpan(0, 0, 30);
            TimeoutTimer.Start();
        }

        #endregion

        #region Commands

        #region Command : Settings

        public DelegateCommand<object> SettingsCommand { get; set; }

        private void OnSettings(object param)
        {
            ShowSettingsModalView();
        }

        #endregion

        #region Command : Generate Report

        public DelegateCommand<object> GenerateReportCommand { get; set; }

        private void OnGenerateReport(object param)
        {
            ShowModalView<GenerateReportView>();
        }

        #endregion

        #region Command : Backup Database

        public DelegateCommand<object> BackupDatabaseCommand { get; set; }

        private void OnBackupDatabase(object param)
        {
            BackupRestoreManager.BackUp();
        }

        #endregion

        #region Command : Restore Database

        public DelegateCommand<object> RestoreDatabaseCommand { get; set; }

        private void OnRestoreDatabase(object param)
        {
            BackupRestoreManager.Restore();
        }

        #endregion

        #region Command : Copy Workshop Card

        public DelegateCommand<object> CopyWorkshopCardCommand { get; set; }

        private void OnCopyWorkshopCard(object param)
        {
        }

        #endregion

        #region Command : Exit Command

        public DelegateCommand<Window> ExitCommand { get; set; }

        private void OnExit(Window window)
        {
            if (window != null)
                window.Close();
        }

        #endregion

        #region Command : New Home Screen Command

        public DelegateCommand<object> HomeScreenCommand { get; set; }

        private void OnHomeScreenCommand(object param)
        {
            ShowView<HomeScreenView>();
        }

        #endregion

        #region Command : New Tachograph Command

        public DelegateCommand<object> NewTachographCommand { get; set; }

        private void OnNewTachograph(object param)
        {
            NewTachographViewModel viewModel = (NewTachographViewModel)ShowView<NewTachographView>();
            viewModel.SetDocumentTypes(true);

        }

        #endregion

        #region Command : New Analogue Tachograph Command

        public DelegateCommand<object> NewAnalogueTachographCommand { get; set; }

        private void OnNewAnalogueTachograph(object param)
        {
            NewTachographViewModel viewModel = (NewTachographViewModel)ShowView<NewAnalogueTachographView>();
            viewModel.SetDocumentTypes(false);
        }

        #endregion

        #region Command : New Undownloadability Command

        public DelegateCommand<object> NewUndownloadabilityCommand { get; set; }

        private void OnNewUndownloadability(object param)
        {
            ShowView<NewUndownloadabilityView>();
        }

        #endregion

        #region Command : Tachograph History Command

        public DelegateCommand<object> TachographHistoryCommand { get; set; }

        private void OnTachographHistory(object param)
        {
            ShowView<TachographHistoryView>();
        }

        #endregion

        #region Command : Undownloadability History Command

        public DelegateCommand<object> UndownloadabilityHistoryCommand { get; set; }

        private void OnUndownloadabilityHistory(object param)
        {
            ShowView<UndownloadabilityHistoryView>();
        }

        #endregion

        #region Command : Calibrations Command

        public DelegateCommand<object> CalibrationsCommand { get; set; }

        private void OnCalibrations(object param)
        {
            ShowView<CalibrationsView>();
        }

        #endregion

        #region Command : Driver Card Files Command

        public DelegateCommand<object> DriverCardFilesCommand { get; set; }

        private void OnDriverCardFiles(object param)
        {
            ShowView<DriverCardFilesView>();
        }

        #endregion

        #region Command : Workshop Card Files Command

        public DelegateCommand<object> WorkshopCardFilesCommand { get; set; }

        private void OnWorkshopCardFiles(object param)
        {
            ShowView<WorkshopCardFilesView>();
        }

        #endregion

        #region Command : Show Window Command

        public DelegateCommand<object> ShowWindowCommand { get; set; }

        private static void OnShowWindow(object obj)
        {
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }

        #endregion

        #region Command : Exit Tray Command

        public DelegateCommand<object> ExitTrayCommand { get; set; }

        private static void OnExitTray(object obj)
        {
            Application.Current.MainWindow.Close();
        }

        #endregion

        #region Command : Cancel

        public DelegateCommand<object> CancelCommand { get; set; }

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

        #endregion

        #region Command : New Speedlimiter

        public DelegateCommand<object> NewSpeedlimiterCommand { get; set; }

        private void OnNewSpeedlimiter(object obj)
        {
            ShowView<SpeedlimiterView>();
        }

        #endregion

        #region Command : View Technical Information

        public DelegateCommand<object> ViewTechnicalInformationCommand { get; set; }

        private static void OnViewTechnicalInformation(object obj)
        {
            ExceptionWindow.ExceptionWindow exceptionWindow = new ExceptionWindow.ExceptionWindow();
            exceptionWindow.ShowDialog();
        }

        #endregion

        #region Command : Update

        public DelegateCommand<object> UpdateCommand { get; set; }

        private void OnUpdate(object arg)
        {
            Process.Start("AutoUpdater.exe", "/checknow");
        }

        #endregion

        #region Command : Save Modal

        public DelegateCommand<object> SaveModalCommand { get; set; }

        private void OnSaveModal(object obj)
        {
            CloseSettingsModal(true);

            INewDocumentViewModel documentViewModel = View.DataContext as INewDocumentViewModel;
            if (documentViewModel != null)
            {
                documentViewModel.OnModalClosed();
            }
        }

        #endregion

        #region Command : Cancel Modal

        public DelegateCommand<object> CancelModalCommand { get; set; }

        private void OnCancelModal(object obj)
        {
            CloseSettingsModal(false);
        }

        #endregion

        #endregion

        #region Public Properties

        public DispatcherTimer TimeoutTimer { get; set; }

        public bool IsLocked { get; set; }

        public bool IsNavigationLocked { get; set; }

        #endregion

        #region Public Methods

        public IViewModel ShowView<T>() where T : UserControl, new()
        {
            return ShowView<T>(this);
        }

        #endregion

        #region Overrides

        protected override void InitialiseCommands()
        {
            SettingsCommand = new DelegateCommand<object>(OnSettings);
            GenerateReportCommand = new DelegateCommand<object>(OnGenerateReport);
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
            CalibrationsCommand = new DelegateCommand<object>(OnCalibrations);
            DriverCardFilesCommand = new DelegateCommand<object>(OnDriverCardFiles);
            WorkshopCardFilesCommand = new DelegateCommand<object>(OnWorkshopCardFiles);
            ShowWindowCommand = new DelegateCommand<object>(OnShowWindow);
            ExitTrayCommand = new DelegateCommand<object>(OnExitTray);
            NewSpeedlimiterCommand = new DelegateCommand<object>(OnNewSpeedlimiter);
            CancelCommand = new DelegateCommand<object>(OnCancel);
            ViewTechnicalInformationCommand = new DelegateCommand<object>(OnViewTechnicalInformation);
            UpdateCommand = new DelegateCommand<object>(OnUpdate);
            SaveModalCommand = new DelegateCommand<object>(OnSaveModal);
            CancelModalCommand = new DelegateCommand<object>(OnCancelModal);
        }

        #endregion

        #region Private Methods

        private void CloseSettingsModal(bool save)
        {
            SettingsView settingsView = ModalView as SettingsView;
            if (settingsView != null)
            {
                SettingsViewModel vm = (SettingsViewModel)settingsView.DataContext;

                try
                {
                    vm.ClearSettingsViewCache(save);
                    vm.OnClosing(!save);
                    vm.Dispose();
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex);
                }
            }
            IsModalWindowVisible = false;
        }

        private void OnTimeoutTimerTick(object sender, EventArgs e)
        {
            if (IsLocked || !UserManagement.HasTimedOut()) 
                return;

            IsLocked = true;
            LogInWindow.LogInWindow logInWindow = new LogInWindow.LogInWindow();
            if (logInWindow.ShowDialog() == true)
            {
                IsLocked = false;
                UserManagement.LastCommandExecuted = DateTime.Now;
            }
            else
                Application.Current.Shutdown(0);
        }

        #endregion
    }
}