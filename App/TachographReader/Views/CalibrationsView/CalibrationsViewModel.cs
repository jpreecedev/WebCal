using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using StructureMap;
using Webcal.DataModel.Library;
using Webcal.Windows.CalibrationDetailsWindow;
using Webcal.Windows.ProgressWindow;
using Webcal.Core;
using Webcal.Library;
using Webcal.Properties;

namespace Webcal.Views
{
    public class CalibrationsViewModel : BaseCardBrowserViewModel
    {
        #region Public Properties

        public ObservableCollection<CalibrationRecord> CalibrationRecords { get; set; }

        public CalibrationRecord SelectedCalibrationRecord { get; set; }

        #endregion

        #region Overrides

        protected override void InitialiseCommands()
        {
            ReadFromCardCommand = new DelegateCommand<object>(OnReadFromCard);
            SelectAllCommand = new DelegateCommand<object>(OnSelectAll);
            ClearSelectionCommand = new DelegateCommand<object>(OnClearSelection);
            ShowDetailsCommand = new DelegateCommand<object>(OnShowDetails);
        }

        #endregion

        #region Commands

        #region Command : Read From Card

        public DelegateCommand<object> ReadFromCardCommand { get; set; }

        private void OnReadFromCard(object obj)
        {
            ProgressWindow window = new ProgressWindow();

            Task<List<CalibrationRecord>> task = new Task<List<CalibrationRecord>>(SmartCardMonitor.Instance.GetCalibrationHistory);
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

        #endregion

        #region Command : Select All

        public DelegateCommand<object> SelectAllCommand { get; set; }

        private void OnSelectAll(object obj)
        {
            if (CalibrationRecords.IsNullOrEmpty())
                return;

            foreach (CalibrationRecord calibrationRecord in CalibrationRecords)
                calibrationRecord.IsSelected = true;
        }

        #endregion

        #region Command : Clear Selection

        public DelegateCommand<object> ClearSelectionCommand { get; set; }

        private void OnClearSelection(object obj)
        {
            if (CalibrationRecords.IsNullOrEmpty())
                return;

            foreach (CalibrationRecord calibrationRecord in CalibrationRecords)
                calibrationRecord.IsSelected = false;
        }

        #endregion

        #region Command : Show Details

        public DelegateCommand<object> ShowDetailsCommand { get; set; }

        private void OnShowDetails(object obj)
        {
            if (SelectedCalibrationRecord == null)
                return;

            CalibrationDetailsWindow window = new CalibrationDetailsWindow();
            ((CalibrationDetailsViewModel)window.DataContext).CalibrationRecord = SelectedCalibrationRecord;
            window.ShowDialog();
        }

        #endregion

        #endregion
    }
}
