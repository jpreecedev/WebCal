using System.Reflection;
using System.Windows.Controls;
using StructureMap;
using Webcal.Core;
using Webcal.DataModel;
using Webcal.DataModel.Core;
using Webcal.Library;
using Webcal.Properties;
using Webcal.Windows.ReprintWindow;

namespace Webcal.Views
{
    public class HomeScreenViewModel : BaseViewModel
    {
        #region Public Properties

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

        #endregion

        #region Overrides

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

        #endregion

        #region Commands

        #region Command : New Digital Tachograph Document

        public DelegateCommand<object> NewDigitalTachographCommand { get; set; }

        private void OnNewDigitalDocument(object obj)
        {
            NewTachographViewModel viewModel = (NewTachographViewModel)MainWindow.ShowView<NewTachographView>();
            viewModel.SetDocumentTypes(true);
        }

        #endregion

        #region Command : New Analogue Tachograph Document

        public DelegateCommand<object> NewAnalogueTachographDocumentCommand { get; set; }

        private void OnNewAnalogueDocument(object param)
        {
            NewTachographViewModel viewModel = (NewTachographViewModel)MainWindow.ShowView<NewAnalogueTachographView>();
            viewModel.SetDocumentTypes(false);
        }

        #endregion

        #region Command : Reprint Label

        public DelegateCommand<object> ReprintLabelCommand { get; set; }

        private void OnReprintLabel(object obj)
        {
            ReprintWindow window = new ReprintWindow();
            window.DataContext = new ReprintWindowViewModel { ReprintMode = ReprintMode.Label };
            window.ShowDialog();
        }

        #endregion

        #region Command : Reprint Certificate

        public DelegateCommand<object> ReprintCertificateCommand { get; set; }

        private void OnReprintCertificate(object obj)
        {
            ReprintWindow window = new ReprintWindow();
            window.DataContext = new ReprintWindowViewModel { ReprintMode = ReprintMode.Certificate };
            window.ShowDialog();
        }

        #endregion

        #region Command : Resize Command

        public DelegateCommand<UserControl> ResizeCommand { get; set; }

        private void OnResize(UserControl userControl)
        {
            if (userControl == null)
                return;

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

        #endregion

        #endregion

        #region Private Methods

       private static bool GetAutoPrintLabels()
       {
           //This is not nice, but is necessary to prevent data loss

           IGeneralSettingsRepository repository = ObjectFactory.GetInstance<IGeneralSettingsRepository>();
           WorkshopSettings workshopSettings = repository.GetSettings();
           return workshopSettings.AutoPrintLabels;
       }

        private void SaveAutoPrintLabels()
        {
            //This is not nice, but is necessary to prevent data loss

            IGeneralSettingsRepository repository = ObjectFactory.GetInstance<IGeneralSettingsRepository>();
            WorkshopSettings workshopSettings = repository.GetSettings();
            workshopSettings.AutoPrintLabels = AutoPrintLabels;
            repository.Save(workshopSettings);
        }

        #endregion
    }
}
