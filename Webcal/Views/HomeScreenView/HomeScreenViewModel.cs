namespace Webcal.Views
{
    using System.Reflection;
    using System.Windows.Controls;
    using Windows.ReprintWindow;
    using Core;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using Library;
    using Properties;
    using Shared;

    public class HomeScreenViewModel : BaseViewModel
    {
        public double ButtonHeight { get; set; }
        public double ButtonWidth { get; set; }
        public string DigitalText { get; set; }
        public string AnalogueText { get; set; }
        public string CertificateText { get; set; }
        public string PlaqueText { get; set; }
        public string ColumnWidth { get; set; }
        public bool AutoPrintLabels { get; set; }

        public string VersionNumber
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                return assembly.GetName().Version.ToString();
            }
        }

        public DelegateCommand<object> NewDigitalTachographCommand { get; set; }
        public DelegateCommand<object> NewAnalogueTachographDocumentCommand { get; set; }
        public DelegateCommand<object> ReprintLabelCommand { get; set; }
        public DelegateCommand<object> ReprintCertificateCommand { get; set; }
        public DelegateCommand<UserControl> ResizeCommand { get; set; }

        protected override void InitialiseCommands()
        {
            NewDigitalTachographCommand = new DelegateCommand<object>(OnNewDigitalDocument);
            NewAnalogueTachographDocumentCommand = new DelegateCommand<object>(OnNewAnalogueDocument);
            ReprintLabelCommand = new DelegateCommand<object>(OnReprintLabel);
            ReprintCertificateCommand = new DelegateCommand<object>(OnReprintCertificate);
            ResizeCommand = new DelegateCommand<UserControl>(OnResize);
        }

        protected override void InitialiseRepositories()
        {
            AutoPrintLabels = GetAutoPrintLabels();
        }

        public override void OnClosing(bool cancelled)
        {
            SaveAutoPrintLabels();
        }

        private void OnNewDigitalDocument(object obj)
        {
            var viewModel = (NewTachographViewModel) MainWindow.ShowView<NewTachographView>();
            viewModel.SetDocumentTypes(true);
        }

        private void OnNewAnalogueDocument(object param)
        {
            var viewModel = (NewTachographViewModel) MainWindow.ShowView<NewAnalogueTachographView>();
            viewModel.SetDocumentTypes(false);
        }

        private void OnReprintLabel(object obj)
        {
            var window = new ReprintWindow();
            window.DataContext = new ReprintWindowViewModel {ReprintMode = ReprintMode.Label};
            window.ShowDialog();
        }

        private void OnReprintCertificate(object obj)
        {
            var window = new ReprintWindow();
            window.DataContext = new ReprintWindowViewModel {ReprintMode = ReprintMode.Certificate};
            window.ShowDialog();
        }

        private void OnResize(UserControl userControl)
        {
            if (userControl == null)
            {
                return;
            }

            if (userControl.ActualHeight < 595)
            {
                ButtonHeight = 150;
                ButtonWidth = 128;

                DigitalText = string.Empty;
                AnalogueText = string.Empty;
                CertificateText = string.Empty;
                PlaqueText = string.Empty;
            }
            else
            {
                ButtonHeight = 275;
                ButtonWidth = 235;

                DigitalText = Resources.TXT_DIGITAL_TACHOGRAPH_DOCUMENT;
                AnalogueText = Resources.TXT_ANALOGUE_TACHOGRAPH_DOCUMENT;
                CertificateText = Resources.TXT_CERTIFICATE_TACHOGRAPH_DOCUMENT;
                PlaqueText = Resources.TXT_PLAQUE_TACHOGRAPH_DOCUMENT;
            }
        }

        private static bool GetAutoPrintLabels()
        {
            //This is not nice, but is necessary to prevent data loss

            var repository = ContainerBootstrapper.Container.GetInstance<ISettingsRepository<WorkshopSettings>>();
            WorkshopSettings workshopSettings = repository.GetWorkshopSettings();
            return workshopSettings.AutoPrintLabels;
        }

        private void SaveAutoPrintLabels()
        {
            //This is not nice, but is necessary to prevent data loss

            var repository = ContainerBootstrapper.Container.GetInstance<ISettingsRepository<WorkshopSettings>>();
            WorkshopSettings workshopSettings = repository.GetWorkshopSettings();
            workshopSettings.AutoPrintLabels = AutoPrintLabels;
            repository.Save(workshopSettings);
        }
    }
}