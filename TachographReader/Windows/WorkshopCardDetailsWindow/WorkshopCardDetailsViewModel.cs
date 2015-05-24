namespace TachographReader.Windows.WorkshopCardDetailsWindow
{
    using System.Windows;
    using Core;
    using DataModel;
    using DataModel.Core;
    using DDDFileReader;
    using Properties;
    using Shared;
    using Shared.Helpers;

    public class WorkshopCardDetailsViewModel : BaseModalWindowViewModel
    {
        public WorkshopCardDetailsViewModel()
        {
            OkCommand = new DelegateCommand<Window>(OnOk);
        }

        public BaseFile WorkshopCardFile { get; set; }
        public DelegateCommand<Window> OkCommand { get; set; }

        public TachographCard TachographCard { get; set; }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == "WorkshopCardFile")
            {
                AsyncHelper.CallAsync(() =>
                {
                    return DDDReader.Read(WorkshopCardFile.SerializedFile);
                },
                result =>
                {
                    TachographCard = result;
                },
                exception =>
                {
                    MessageBoxHelper.ShowError(string.Format("{0}\n\n{1}", Resources.TXT_ERROR_WHILST_READING_WORKSHOP_CARD, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, exception)));
                },
                () =>
                {
                    CloseProgressWindow();
                });

                ShowProgressWindow();
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