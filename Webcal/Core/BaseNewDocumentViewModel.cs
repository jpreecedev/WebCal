﻿namespace Webcal.Core
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using EventArguments;
    using Library;
    using Library.PDF;
    using Properties;
    using Shared;
    using Views;

    public class BaseNewDocumentViewModel : BaseMainViewModel, INewDocumentViewModel
    {
        public DelegateCommand<Grid> ExportPDFCommand { get; set; }
        public DelegateCommand<Grid> PrintCommand { get; set; }
        public DelegateCommand<string> RegistrationChangedCommand { get; set; }
        public WorkshopSettings WorkshopSettings { get; set; }
        public MailSettings MailSettings { get; set; }
        public bool IsHistoryMode { get; set; }
        public IDriverCardReader DriverCardReader { get; set; }

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

        protected override void InitialiseRepositories()
        {
            base.InitialiseRepositories();

            WorkshopSettings = ContainerBootstrapper.Container.GetInstance<ISettingsRepository<WorkshopSettings>>().GetWorkshopSettings();
            MailSettings = ContainerBootstrapper.Container.GetInstance<ISettingsRepository<MailSettings>>().Get();
        }

        protected override void Load()
        {
            DriverCardReader = new DriverCardReader();

            DriverCardReader.Completed += (sender, args) =>
            {
                if (args.Operation == SmartCardReadOperation.Fast)
                {
                    OnFastReadCompleted(sender, args);
                }
                else
                {
                    OnDumpCompleted(sender, args);
                }
            };

            DriverCardReader.Progress += OnProgress;
        }

        protected virtual void Add()
        {
        }

        protected virtual void RegistrationChanged(string registrationNumber)
        {
        }

        protected virtual void OnProgress(object sender, DriverCardProgressEventArgs e)
        {
        }

        protected virtual void OnDumpCompleted(object sender, DriverCardCompletedEventArgs e)
        {
        }

        protected virtual void OnFastReadCompleted(object sender, DriverCardCompletedEventArgs e)
        {
        }

        public override void OnClosing(bool cancelled)
        {
            DriverCardReader.Dispose();
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
                if (PDFHelper.GenerateTachographPlaque(document, false, IsHistoryMode))
                {
                    if (!IsHistoryMode)
                    {
                        EmailHelper.SendEmail(WorkshopSettings, MailSettings, document, PDFHelper.LastPDFOutputPath);
                        Add();
                    }

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
            if (PDFHelper.GenerateTachographPlaque(document, true, IsHistoryMode))
            {
                try
                {
                    PDFHelper.Print(Path.Combine(DocumentHelper.GetTemporaryDirectory(), "document.pdf"));
                    if (!IsHistoryMode)
                    {
                        EmailHelper.SendEmail(WorkshopSettings, MailSettings, document, Path.Combine(DocumentHelper.GetTemporaryDirectory(), "document.pdf"));
                        Add();
                    }
                }
                finally
                {
                    Close();
                }
            }
        }

        private void OnRegistrationChanged(string registrationNumber)
        {
            if (string.IsNullOrEmpty(registrationNumber) || registrationNumber.Length < 3)
            {
                return;
            }

            if (DriverCardReader != null)
            {
                RegistrationChanged(registrationNumber);
            }
        }

        private static Document GetNewDocument(FrameworkElement root)
        {
            var sender = root.DataContext as BaseNewDocumentViewModel;
            if (sender == null)
            {
                return null;
            }

            var viewModel = sender as NewTachographViewModel;
            if (viewModel != null)
            {
                return viewModel.Document;
            }

            return ((NewUndownloadabilityViewModel) sender).Document;
        }

        private void Close()
        {
            MainWindow.IsNavigationLocked = false;
            MainWindow.ShowView<HomeScreenView>();
        }
    }
}