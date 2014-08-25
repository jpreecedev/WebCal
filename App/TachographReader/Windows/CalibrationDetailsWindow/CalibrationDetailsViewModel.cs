namespace Webcal.Windows.CalibrationDetailsWindow
{
    using System.Windows;
    using Core;
    using DataModel.Library;

    public class CalibrationDetailsViewModel : BaseNotification
    {
        public CalibrationDetailsViewModel()
        {
            OkCommand = new DelegateCommand<Window>(OnOk);
        }
        
        public CalibrationRecord CalibrationRecord { get; set; }
        
        public DelegateCommand<Window> OkCommand { get; set; }

        private void OnOk(Window window)
        {
            if (window == null)
                return;

            window.Close();
        }
    }
}