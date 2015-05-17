namespace TachographReader.Windows.CalibrationDetailsWindow
{
    using System.Windows;
    using Connect.Shared.Models;
    using Core;

    public class CalibrationDetailsViewModel : BaseModalWindowViewModel
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
            {
                return;
            }

            window.Close();
        }
    }
}