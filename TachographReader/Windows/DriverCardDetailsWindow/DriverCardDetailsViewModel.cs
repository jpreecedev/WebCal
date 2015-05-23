namespace TachographReader.Windows.DriverCardDetailsWindow
{
    using System.Windows;
    using Core;
    using DataModel;
    using DDDFileReader;

    public class DriverCardDetailsViewModel : BaseModalWindowViewModel
    {
        public DriverCardDetailsViewModel()
        {
            OkCommand = new DelegateCommand<Window>(OnOk);
        }

        public BaseFile DriverCardFile { get; set; }
        public DelegateCommand<Window> OkCommand { get; set; }

        public TachographCard TachographCard { get; set; }
        public ActivityDataItem SelectedActivityDataItem { get; set; }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == "DriverCardFile")
            {
                TachographCard = DDDReader.Read(DriverCardFile.SerializedFile);                
            }
        }

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