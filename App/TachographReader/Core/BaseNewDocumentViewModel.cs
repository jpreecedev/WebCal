using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Webcal.DataModel;
using Webcal.Library;
using Webcal.Library.PDF;
using Webcal.Properties;
using Webcal.Shared;
using Webcal.Views;

namespace Webcal.Core
{
    public class BaseNewDocumentViewModel : BaseMainViewModel, INewDocumentViewModel
    {
        #region Protected Properties

        protected SmartCardMonitor SmartCardReader
        {
            get { return SmartCardMonitor.Instance; }
        }

        #endregion

        #region Overrides

        public virtual void OnModalClosed()
        {
        }

        protected override void InitialiseCommands()
        {
            base.InitialiseCommands();

            ExportPDFCommand = new DelegateCommand<Grid>(OnExportPDF);
            PrintCommand = new DelegateCommand<Grid>(OnPrint);
            RegistrationChangedCommand = new DelegateCommand<string>(OnRegistrationChanged);
        }

        protected virtual void Add()
        {
        }

        protected virtual void RegistrationChanged(string registrationNumber)
        {

        }

        #endregion

        #region Commands

        #region Command : Export PDF

        public DelegateCommand<Grid> ExportPDFCommand { get; set; }

        private void OnExportPDF(Grid root)
        {
            if (!IsValid(root))
            {
                ShowError(Resources.EXC_MISSING_FIELDS);
                return;
            }

            try
            {
                Document document = GetNewDocument(root);
                if (PDFHelper.GenerateTachographPlaque(document, false))
                {
                    EmailHelper.SendEmail(document, PDFHelper.LastPDFOutputPath);

                    Add();
                    Close();
                }
            }
            catch (Exception ex)
            {
                ShowError(Resources.ERR_UNABLE_GENERATE_PDF, ExceptionPolicy.HandleException(ex));
            }
        }

        #endregion

        #region Command : Print

        public DelegateCommand<Grid> PrintCommand { get; set; }

        private void OnPrint(Grid root)
        {
            if (!IsValid(root))
            {
                ShowError(Resources.EXC_MISSING_FIELDS);
                return;
            }

            Document document = GetNewDocument(root);
            if (PDFHelper.GenerateTachographPlaque(document, true))
            {
                PDFHelper.Print(Path.Combine(DocumentHelper.GetTemporaryDirectory(), "document.pdf"));
                EmailHelper.SendEmail(document, Path.Combine(DocumentHelper.GetTemporaryDirectory(), "document.pdf"));

                Add();
                Close();
            }
        }

        #endregion

        #region Command : Registration Changed

        public DelegateCommand<string> RegistrationChangedCommand { get; set; }

        private void OnRegistrationChanged(string registrationNumber)
        {
            if (string.IsNullOrEmpty(registrationNumber) || registrationNumber.Length < 3)
                return;

            if (SmartCardReader != null)
                RegistrationChanged(registrationNumber);
        }

        #endregion

        #endregion

        #region Private Methods

        private static Document GetNewDocument(FrameworkElement root)
        {
            BaseNewDocumentViewModel sender = root.DataContext as BaseNewDocumentViewModel;
            if (sender == null)
                return null;

            NewTachographViewModel viewModel = sender as NewTachographViewModel;
            if (viewModel != null)
            {
                return viewModel.Document;
            }

            return ((NewUndownloadabilityViewModel)sender).Document;
        }

        private void Close()
        {
            MainWindow.IsNavigationLocked = false;
            MainWindow.ShowView<HomeScreenView>();
        }

        #endregion
    }
}