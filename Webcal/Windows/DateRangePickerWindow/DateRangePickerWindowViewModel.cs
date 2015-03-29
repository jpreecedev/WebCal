namespace TachographReader.Windows.DateRangePickerWindow
{
    using System;
    using System.Windows;
    using Core;

    public class DateRangePickerWindowViewModel : BaseModalWindowViewModel
    {
        public DateRangePickerWindowViewModel()
        {
            SelectCommand = new DelegateCommand<Window>(OnSelect);
            CancelCommand = new DelegateCommand<Window>(OnCancel);

            StartDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            EndDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
        }

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DelegateCommand<Window> SelectCommand { get; set; }
        public DelegateCommand<Window> CancelCommand { get; set; }

        private void OnSelect(Window window)
        {
            window.DialogResult = true;
            window.Close();
        }

        private void OnCancel(Window window)
        {
            window.DialogResult = false;
            window.Close();
        }
    }
}