namespace TachographReader.Core
{
    using System;
    using System.Windows.Controls;
    using Connect.Shared;
    using Connect.Shared.Models;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using EventArguments;
    using Library;
    using Library.PDF;
    using Properties;
    using Shared;
    using Views;

    public class BaseNewDocumentViewModel : BaseMainViewModel
    {
        public bool IsHistoryMode { get; set; }
        public bool IsReadOnly { get; set; }
    }

    public class BaseNewDocumentViewModel<T> : BaseNewDocumentViewModel, INewDocumentViewModel where T : class
    {
        public DelegateCommand<Grid> ExportPDFCommand { get; set; }
        public DelegateCommand<Grid> PrintCommand { get; set; }
        public DelegateCommand<string> RegistrationChangedCommand { get; set; }
        public DelegateCommand<string> TachographMakeChangedCommand { get; set; }

        public WorkshopSettings WorkshopSettings { get; set; }
        public MailSettings MailSettings { get; set; }
        public RegistrationData RegistrationData { get; set; }
        public IDriverCardReader DriverCardReader { get; set; }

        public bool IsSearchingConnect { get; set; }
        public bool IsRegistrationChanging { get; set; }
        public bool CardBeingRead { get; set; }

        public T Document { get; set; }

        public virtual void OnModalClosed()
        {
        }

        protected virtual void Close()
        {
            MainWindow.ShowView<HomeScreenView>();
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
            DriverCardReader.CardInserted += OnCardInserted;
            DriverCardReader.CardRemoved += OnCardRemoved;
        }

        protected virtual void Add()
        {

        }

        protected virtual void Update()
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

        protected virtual void OnCardInserted(object sender, EventArgs e)
        {
        }

        protected virtual void OnCardRemoved(object sender, EventArgs e)
        {
        }

        protected virtual void TachographMakeChanged(string make)
        {

        }

        protected virtual Document GetDocument()
        {
            return null;
        }

        protected virtual BaseReport GetReport()
        {
            return null;
        }

        public override void OnClosing(bool cancelled)
        {
            if (DriverCardReader != null)
            {
                DriverCardReader.CardInserted -= OnCardInserted;
                DriverCardReader.CardRemoved -= OnCardRemoved;
                DriverCardReader.Progress -= OnProgress;
                DriverCardReader.CardRemoved -= OnCardRemoved;
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
            if (CardBeingRead)
            {
                ShowWarning(Resources.TXT_CARD_READ_IN_PROGRESS, Resources.TXT_CARD_READ_IN_PROGRESS_CAPTION);
                return;
            }

            try
            {
                Document document;
                BaseReport report;
                var pdfDocumentResult = ToPDF(out document, out report, false, true);

                if (!pdfDocumentResult.Success)
                {
                    return;
                }

                if (!IsHistoryMode)
                {
                    try
                    {
                        var miscellaneousSettings = ContainerBootstrapper.Resolve<ISettingsRepository<MiscellaneousSettings>>().GetMiscellaneousSettings();
                        if (document != null)
                        {
                            document.ToPDF(IsHistoryMode, miscellaneousSettings.ExcludeLogosWhenPrinting).Email(WorkshopSettings, MailSettings);

                            if (document is TachographDocument && ((TachographDocument)document).IsDigital && IMSWarningHelper.IsVehicleAffected(document.RegistrationNumber))
                            {
                                ShowMessage(DataModel.Properties.Resources.TXT_IMS_WARNING_MESSAGE, DataModel.Properties.Resources.TXT_IMS_WARNING_TITLE);
                            }
                        }
                        if (report != null)
                        {
                            report.ToReportPDF(IsHistoryMode, miscellaneousSettings.ExcludeLogosWhenPrinting).Email(WorkshopSettings, MailSettings);
                        }
                    }
                    catch
                    {

                    }

                    Add();
                }
                else
                {
                    Update();
                }

                Close();
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
            if (CardBeingRead)
            {
                ShowWarning(Resources.TXT_CARD_READ_IN_PROGRESS, Resources.TXT_CARD_READ_IN_PROGRESS_CAPTION);
                return;
            }

            var miscellaneousSettings = GetInstance<ISettingsRepository<MiscellaneousSettings>>().GetMiscellaneousSettings();

            Document document;
            BaseReport report;
            ToPDF(out document, out report, miscellaneousSettings.ExcludeLogosWhenPrinting, false).Print();

            if (!IsHistoryMode)
            {
                try
                {
                    ToPDF(out document, out report, false, false).Email(WorkshopSettings, MailSettings);

                    if (document is TachographDocument && ((TachographDocument)document).IsDigital && IMSWarningHelper.IsVehicleAffected(document.RegistrationNumber))
                    {
                        ShowMessage(DataModel.Properties.Resources.TXT_IMS_WARNING_MESSAGE, DataModel.Properties.Resources.TXT_IMS_WARNING_TITLE);
                    }
                }
                finally
                {
                    Add();
                }
            }
            else
            {
                Update();
            }

            Close();
        }

        private PDFDocumentResult ToPDF(out Document document, out BaseReport report, bool excludeLogos, bool promptUser)
        {
            document = GetDocument();
            report = GetReport();

            if (document != null)
            {
                return document.ToPDF(IsHistoryMode, excludeLogos, promptUser);
            }
            return report.ToReportPDF(IsHistoryMode, excludeLogos, promptUser);
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

                RegistrationChanged(registrationNumber);

                IsRegistrationChanging = false;
            }
        }
    }
}