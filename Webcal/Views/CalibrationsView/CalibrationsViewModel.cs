namespace Webcal.Views
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.CalibrationDetailsWindow;
    using Windows.ProgressWindow;
    using Core;
    using DataModel.Library;
    using Library;
    using Properties;

    public class CalibrationsViewModel : BaseCardBrowserViewModel
    {
        public ObservableCollection<CalibrationRecord> CalibrationRecords { get; set; }
        public CalibrationRecord SelectedCalibrationRecord { get; set; }
        public DelegateCommand<object> ReadFromCardCommand { get; set; }
        public DelegateCommand<object> SelectAllCommand { get; set; }
        public DelegateCommand<object> ClearSelectionCommand { get; set; }
        public DelegateCommand<object> ShowDetailsCommand { get; set; }

        protected override void InitialiseCommands()
        {
            ReadFromCardCommand = new DelegateCommand<object>(OnReadFromCard);
            SelectAllCommand = new DelegateCommand<object>(OnSelectAll);
            ClearSelectionCommand = new DelegateCommand<object>(OnClearSelection);
            ShowDetailsCommand = new DelegateCommand<object>(OnShowDetails);
        }

        private void OnReadFromCard(object obj)
        {
            var window = new ProgressWindow();

            var task = new Task<List<CalibrationRecord>>(SmartCardMonitor.Instance.GetCalibrationHistory);
            task.Start();
            task.ContinueWith(t =>
            {
                window.Close();
                if (t.Result.IsNullOrEmpty())
                {
                    ShowMessage(Resources.ERR_UNABLE_GET_CALIBRATION_HISTORY, Resources.TXT_INFORMATION);
                    return;
                }
                CalibrationRecords = new ObservableCollection<CalibrationRecord>(t.Result.OrderByDescending(c => c.CalibrationTime));
            },
                TaskScheduler.FromCurrentSynchronizationContext());

            window.ShowDialog();
        }

        private void OnSelectAll(object obj)
        {
            if (CalibrationRecords.IsNullOrEmpty())
            {
                return;
            }

            foreach (CalibrationRecord calibrationRecord in CalibrationRecords)
            {
                calibrationRecord.IsSelected = true;
            }
        }

        private void OnClearSelection(object obj)
        {
            if (CalibrationRecords.IsNullOrEmpty())
            {
                return;
            }

            foreach (CalibrationRecord calibrationRecord in CalibrationRecords)
            {
                calibrationRecord.IsSelected = false;
            }
        }

        private void OnShowDetails(object obj)
        {
            if (SelectedCalibrationRecord == null)
            {
                return;
            }

            var window = new CalibrationDetailsWindow();
            ((CalibrationDetailsViewModel) window.DataContext).CalibrationRecord = SelectedCalibrationRecord;
            window.ShowDialog();
        }
    }
}