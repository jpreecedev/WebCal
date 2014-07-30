using System.Windows;
using Webcal.Core;
using Webcal.DataModel.Library;

namespace Webcal.Windows.CalibrationDetailsWindow
{
    public class CalibrationDetailsViewModel : BaseNotification
    {
        #region Constructor

        public CalibrationDetailsViewModel()
        {
            OkCommand = new DelegateCommand<Window>(OnOk);
        }

        #endregion

        #region Public Properties

        public CalibrationRecord CalibrationRecord { get; set; }

        #endregion

        #region Commands

        #region Command : Ok

        public DelegateCommand<Window> OkCommand { get; set; }

        private void OnOk(Window window)
        {
            if (window == null)
                return;

            window.Close();
        }

        #endregion

        #endregion
    }
}
