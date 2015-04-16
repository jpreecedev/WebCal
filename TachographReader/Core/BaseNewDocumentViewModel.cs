namespace TachographReader.Core
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Connect.Shared.Models;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using EventArguments;
    using Library;
    using Library.PDF;
    using Properties;
    using Shared;
    using Shared.Connect;
    using Views;
    using DocumentType = Connect.Shared.DocumentType;

    public class BaseNewDocumentViewModel : BaseMainViewModel, INewDocumentViewModel
    {
        public DelegateCommand<Grid> ExportPDFCommand { get; set; }
        public DelegateCommand<Grid> PrintCommand { get; set; }
        public DelegateCommand<string> RegistrationChangedCommand { get; set; }
        public DelegateCommand<string> TachographMakeChangedCommand { get; set; }

        public WorkshopSettings WorkshopSettings { get; set; }
        public MailSettings MailSettings { get; set; }
        public RegistrationData RegistrationData { get; set; }
        public bool IsHistoryMode { get; set; }
        public IDriverCardReader DriverCardReader { get; set; }

        public bool IsSearchingConnect { get; set; }
        public bool IsRegistrationChanging { get; set; }

        public virtual void OnModalClosed()
        {
        }

        protected override void InitialiseCommands()
        {
            base.InitialiseCommands();

            ExportPDFCommand = new DelegateCommand<Grid>(OnExportPDF);
            PrintCommand = new DelegateCommand<Grid>(OnPrint);
            RegistrationChangedCommand = new DelegateCommand<string>(OnRegistrationChanged);
            TachographMakeChangedCommand = new DelegateCommand<string>(TachographMakeChanged);
        }

        protected override void InitialiseRepositories()
        {
            base.InitialiseRepositories();

            WorkshopSettings = GetInstance<ISettingsRepository<WorkshopSettings>>().GetWorkshopSettings();
            MailSettings = GetInstance<ISettingsRepository<MailSettings>>().Get();
            RegistrationData = GetInstance<IRepository<RegistrationData>>().First();
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

        protected virtual bool RegistrationChanged(string registrationNumber)
        {
            return false;
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

        protected virtual void OnFoundDocumentOnConnect(Document document)
        {
            
        }

        protected virtual void TachographMakeChanged(string make)
        {

        }

        protected virtual DocumentType GetDocumentType()
        {
            return DocumentType.Tachograph | DocumentType.Undownloadability | DocumentType.LetterForDecommissioning;
        }

        public override void OnClosing(bool cancelled)
        {
            if (DriverCardReader != null)
            {
                DriverCardReader.Dispose();
            }
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

                var pdfDocumentResult = document.ToPDF(IsHistoryMode, false, true);

                if (pdfDocumentResult.Success)
                {
                    if (!IsHistoryMode)
                    {
                        try
                        {
                            var miscellaneousSettings = ContainerBootstrapper.Resolve<ISettingsRepository<MiscellaneousSettings>>().GetMiscellaneousSettings();
                            document.ToPDF(IsHistoryMode, miscellaneousSettings.ExcludeLogosWhenPrinting).Email(WorkshopSettings, MailSettings);
                        }
                        catch
                        {

                        }

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

            MiscellaneousSettings miscellaneousSettings = GetInstance<ISettingsRepository<MiscellaneousSettings>>().GetMiscellaneousSettings();
            Document document = GetNewDocument(root);

            document.ToPDF(IsHistoryMode, miscellaneousSettings.ExcludeLogosWhenPrinting).Print();

            if (!IsHistoryMode)
            {
                try
                {
                    document.ToPDF(IsHistoryMode).Email(WorkshopSettings, MailSettings);
                }
                finally
                {
                    Add();
                }
            }

            Close();
        }

        private void OnRegistrationChanged(string registrationNumber)
        {
            if (string.IsNullOrEmpty(registrationNumber) || registrationNumber.Length < 3)
            {
                return;
            }

            if (DriverCardReader != null)
            {
                IsRegistrationChanging = true;

                bool foundDocument = RegistrationChanged(registrationNumber);
                if (!foundDocument && RegistrationData.IsConnectEnabled)
                {
                    IsSearchingConnect = true;

                    GetInstance<IConnectClient>().CallAsync(ConnectHelper.GetConnectKeys(), client =>
                    {
                        return client.Service.Find(registrationNumber, GetDocumentType());
                    },
                    result =>
                    {
                        if (result.IsSuccess && result.Data != null)
                        {
                            var documentFound = (Document) result.Data;
                            documentFound.Id = 0;
                            OnFoundDocumentOnConnect(documentFound);
                        }
                    },
                    alwaysCall: () =>
                    {
                        IsSearchingConnect = false;
                        IsRegistrationChanging = false;
                    });
                }

                if (!RegistrationData.IsConnectEnabled)
                {
                    IsRegistrationChanging = false;
                }
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

            LetterForDecommissioningViewModel letterForDecommissioningViewModel = sender as LetterForDecommissioningViewModel;
            if (letterForDecommissioningViewModel != null)
            {
                return letterForDecommissioningViewModel.Document;
            }

            return ((NewUndownloadabilityViewModel)sender).Document;
        }

        private void Close()
        {
            MainWindow.IsNavigationLocked = false;
            MainWindow.ShowView<HomeScreenView>();
        }
    }
}