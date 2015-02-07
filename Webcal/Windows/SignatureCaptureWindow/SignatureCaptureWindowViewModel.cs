namespace Webcal.Windows.SignatureCaptureWindow
{
    using System;
    using System.Drawing;
    using System.Windows;
    using Connect.Shared.Models;
    using Core;
    using DataModel.Core;
    using Library;
    using Properties;
    using Shared;
    using Shared.Helpers;

    public class SignatureCaptureWindowViewModel : BaseNotification
    {
        public SignatureCaptureWindowViewModel()
        {
            InitialiseCommands();
        }

        public DelegateCommand<Window> CloseCommand { get; set; }

        public DelegateCommand<Window> SaveCommand { get; set; }

        public DelegateCommand<object> BrowseCommand { get; set; }

        public Action<Image> OnSignatureCaptured { get; set; }

        public Image SignatureImage { get; set; }

        private void OnSave(Window window)
        {
            if (window != null)
            {
                window.Close();
            }

            if (OnSignatureCaptured != null)
            {
                OnSignatureCaptured(SignatureImage);
            }
        }

        private static void OnClose(Window window)
        {
            if (window == null)
            {
                return;
            }

            window.Close();
        }

        private void OnBrowse(object arg)
        {
            DialogHelperResult result = DialogHelper.OpenFile(DialogFilter.JPG, string.Empty);
            if (result.Result == true)
            {
                try
                {
                    SignatureImage = ImageHelper.LoadImageSafely(result.FileName);

                    try
                    {
                        string path = Imaging.Signature.Transform(new Bitmap(SignatureImage));
                        SignatureImage = ImageHelper.LoadImageSafely(path);
                    }
                    catch (Exception ex)
                    {
                        MessageBoxHelper.ShowError(string.Format("{0}\n\n{1}", Resources.ERR_UNABLE_TO_APPLY_TRANSFORMATIONS, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex)));
                    }
                }
                catch
                {
                    MessageBoxHelper.ShowError(Resources.ERR_UNABLE_TO_LOAD_THE_SPECIFIED_FILE);
                }
            }
        }

        private void InitialiseCommands()
        {
            BrowseCommand = new DelegateCommand<object>(OnBrowse);
            CloseCommand = new DelegateCommand<Window>(OnClose);
            SaveCommand = new DelegateCommand<Window>(OnSave);
        }
    }
}