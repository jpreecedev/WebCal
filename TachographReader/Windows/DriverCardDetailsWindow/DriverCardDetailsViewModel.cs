﻿namespace TachographReader.Windows.DriverCardDetailsWindow
{
    using System.Windows;
    using Core;
    using DataModel;
    using DataModel.Core;
    using DDDFileReader;
    using Properties;
    using Shared;
    using Shared.Helpers;

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
                AsyncHelper.CallAsync(() =>
                {
                    return DDDReader.Read(DriverCardFile.SerializedFile);
                },
                result =>
                {
                    TachographCard = result;
                },
                exception =>
                {
                    MessageBoxHelper.ShowError(string.Format("{0}\n\n{1}", Resources.TXT_ERROR_WHILST_READING_DRIVER_CARD, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, exception)));
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