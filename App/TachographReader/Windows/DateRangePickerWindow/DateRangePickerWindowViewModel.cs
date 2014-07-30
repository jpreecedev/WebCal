using System;
using System.Windows;
using Webcal.Core;

namespace Webcal.Windows.DateRangePickerWindow
{
    public class DateRangePickerWindowViewModel : BaseViewModel
    {
        #region Constructor

        public DateRangePickerWindowViewModel()
        {
            SelectCommand = new DelegateCommand<Window>(OnSelect);
            CancelCommand = new DelegateCommand<Window>(OnCancel);

            StartDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            EndDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
        }

        #endregion

        #region Public Properties

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        #endregion


        #region Command : Select

        public DelegateCommand<Window> SelectCommand { get; set; }

        private void OnSelect(Window window)
        {
            window.DialogResult = true;
            window.Close();
        }

        #endregion


        #region Command : Cancel

        public DelegateCommand<Window> CancelCommand { get; set; }

        private void OnCancel(Window window)
        {
            window.DialogResult = false;
            window.Close();
        }

        #endregion

    }
}
