namespace TachographReader.Views
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Windows.CalibrationDetailsWindow;
    using Windows.ProgressWindow;
    using Connect.Shared.Models;
    using Core;
    using EventArguments;
    using Library;
    using Properties;

    public class CalibrationsViewModel : BaseCardBrowserViewModel
    {
        private ProgressWindow _progressWindow;
        public ObservableCollection<CalibrationRecord> CalibrationRecords { get; set; }
        public CalibrationRecord SelectedCalibrationRecord { get; set; }
        public DelegateCommand<object> ReadFromCardCommand { get; set; }
        public DelegateCommand<object> ShowDetailsCommand { get; set; }
        public DelegateCommand<object> CreateCertificateCommand { get; set; }
        public IDriverCardReader DriverCardReader { get; set; }

        protected override void InitialiseCommands()
        {
            ReadFromCardCommand = new DelegateCommand<object>(OnReadFromCard);
            ShowDetailsCommand = new DelegateCommand<object>(OnShowDetails);
            CreateCertificateCommand = new DelegateCommand<object>(OnCreateCertificate);
        }

        protected override void Load()
        {
            DriverCardReader = new DriverCardReader();
            DriverCardReader.Progress += Progress;
            DriverCardReader.Completed += Completed;
        }

        public override void OnClosing(bool cancelled)
        {
            DriverCardReader.Dispose();
        }

        private void OnReadFromCard(object obj)
        {
            _progressWindow = new ProgressWindow();

            DriverCardReader.GetFullHistory();

            _progressWindow.ShowDialog();
        }

        private void OnShowDetails(object obj)
        {
            if (SelectedCalibrationRecord == null)
            {
                return;
            }

            var window = new CalibrationDetailsWindow();
            ((CalibrationDetailsViewModel)window.DataContext).CalibrationRecord = SelectedCalibrationRecord;
            window.ShowDialog();
        }

        private void OnCreateCertificate(object obj)
        {
            if (SelectedCalibrationRecord == null)
            {
                return;
            }

            var document = TachographDocumentHelper.Create(SelectedCalibrationRecord);

            var viewModel = (NewTachographViewModel)MainWindow.ShowView<NewTachographView>();
            viewModel.SetDocumentTypes(true);
            viewModel.Document = document;
            
            if (SelectedCalibrationRecord.CalibrationTime != null && SelectedCalibrationRecord.CalibrationTime.GetValueOrDefault() != default(DateTime))
            {
                viewModel.Document.Created = SelectedCalibrationRecord.CalibrationTime.GetValueOrDefault();
            }
        }

        private void Progress(object sender, DriverCardProgressEventArgs e)
        {
            ((ProgressWindowViewModel)_progressWindow.DataContext).ProgressText = e.Message;
        }

        private void Completed(object sender, DriverCardCompletedEventArgs e)
        {
            _progressWindow.Close();

            if (!e.IsSuccess)
            {
                ShowMessage(Resources.ERR_UNABLE_GET_CALIBRATION_HISTORY, Resources.TXT_INFORMATION);
                return;
            }

            CalibrationRecords = new ObservableCollection<CalibrationRecord>(e.CalibrationHistory.OrderByDescending(c => c.CalibrationTime));
        }
    }
}