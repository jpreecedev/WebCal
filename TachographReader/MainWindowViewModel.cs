using System.Windows;
using TachographReader.Core;
using TachographReader.Views;

namespace TachographReader
{
    public class MainWindowViewModel : BaseNavigationViewModel
    {
        #region Constructor

        public MainWindowViewModel()
        {
            ShowView<NewTachographView>();
            InitialiseCommands();
        }

        #endregion

        #region Commands

        #region Command : Settings

        public DelegateCommand<object> SettingsCommand { get; set; }

        private void OnSettings(object param)
        {
            ShowModalView<SettingsView>();
        }

        #endregion

        #region Command : Generate Report

        public DelegateCommand<object> GenerateReportCommand { get; set; }

        private void OnGenerateReport(object param)
        {

        }

        #endregion

        #region Command : Backup Database

        public DelegateCommand<object> BackupDatabaseCommand { get; set; }

        private void OnBackupDatabase(object param)
        {

        }

        #endregion

        #region Command : Restore Database

        public DelegateCommand<object> RestoreDatabaseCommand { get; set; }

        private void OnRestoreDatabase(object param)
        {

        }

        #endregion

        #region Command : Copy Workshop Card

        public DelegateCommand<object> CopyWorkshopCardCommand { get; set; }

        private void OnCopyWorkshopCard(object param)
        {

        }

        #endregion

        #region Command : Minimize Command

        public DelegateCommand<Window> MinimizeCommand { get; set; }

        private void OnMinimize(Window param)
        {
            if (param != null)
                param.WindowState = WindowState.Minimized;
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

        #region Command : New Tachograph Command

        public DelegateCommand<object> NewTachographCommand { get; set; }

        private void OnNewTachograph(object param)
        {
            ShowView<NewTachographView>();
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

        #region Command : Tachograph Files Command

        public DelegateCommand<object> TachographFilesCommand { get; set; }

        private void OnTachographFiles(object param)
        {
            ShowView<TachographFilesView>();
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

        #region Command : Close Modal Command

        public DelegateCommand<object> CloseModalCommand { get; set; }

        private void OnCloseModal(object param)
        {
            IsModalWindowVisible = false;
        }

        #endregion

        #endregion

        #region Private Methods

        private void InitialiseCommands()
        {
            SettingsCommand = new DelegateCommand<object>(OnSettings);
            GenerateReportCommand = new DelegateCommand<object>(OnGenerateReport);
            BackupDatabaseCommand = new DelegateCommand<object>(OnBackupDatabase);
            RestoreDatabaseCommand = new DelegateCommand<object>(OnRestoreDatabase);
            CopyWorkshopCardCommand = new DelegateCommand<object>(OnCopyWorkshopCard);
            MinimizeCommand = new DelegateCommand<Window>(OnMinimize);
            ExitCommand = new DelegateCommand<Window>(OnExit);
            NewTachographCommand = new DelegateCommand<object>(OnNewTachograph);
            NewUndownloadabilityCommand = new DelegateCommand<object>(OnNewUndownloadability);
            TachographHistoryCommand = new DelegateCommand<object>(OnTachographHistory);
            UndownloadabilityHistoryCommand = new DelegateCommand<object>(OnUndownloadabilityHistory);
            CalibrationsCommand = new DelegateCommand<object>(OnCalibrations);
            TachographFilesCommand = new DelegateCommand<object>(OnTachographFiles);
            DriverCardFilesCommand = new DelegateCommand<object>(OnDriverCardFiles);
            WorkshopCardFilesCommand = new DelegateCommand<object>(OnWorkshopCardFiles);
            CloseModalCommand = new DelegateCommand<object>(OnCloseModal);
        }

        #endregion
    }
}
