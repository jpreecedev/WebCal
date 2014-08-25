namespace Webcal.Core
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using DataModel;
    using DataModel.Core;
    using Library;
    using Library.PDF;
    using Properties;
    using Shared;
    using Views;

    public class BaseNewDocumentViewModel : BaseMainViewModel, INewDocumentViewModel
    {
        protected SmartCardMonitor SmartCardReader
        {
            get { return SmartCardMonitor.Instance; }
        }

        public DelegateCommand<Grid> ExportPDFCommand { get; set; }
        public DelegateCommand<Grid> PrintCommand { get; set; }
        public DelegateCommand<string> RegistrationChangedCommand { get; set; }


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
                ShowError(Resources.ERR_UNABLE_GENERATE_PDF, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex));
            }
        }

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

        private void OnRegistrationChanged(string registrationNumber)
        {
            if (string.IsNullOrEmpty(registrationNumber) || registrationNumber.Length < 3)
                return;

            if (SmartCardReader != null)
                RegistrationChanged(registrationNumber);
        }
        
        private static Document GetNewDocument(FrameworkElement root)
        {
            var sender = root.DataContext as BaseNewDocumentViewModel;
            if (sender == null)
                return null;

            var viewModel = sender as NewTachographViewModel;
            if (viewModel != null)
                return viewModel.Document;

            return ((NewUndownloadabilityViewModel) sender).Document;
        }

        private void Close()
        {
            MainWindow.IsNavigationLocked = false;
            MainWindow.ShowView<HomeScreenView>();
        }
    }
}