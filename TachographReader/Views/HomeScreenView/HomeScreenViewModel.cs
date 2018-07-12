namespace TachographReader.Views
{
    using System.Reflection;
    using System.Windows;
    using Connect.Shared;
    using Core;
    using DataModel.Library;
    using Library;
    using Library.ViewModels;
    using Properties;
    using Shared;
    using System.Windows.Controls;
    using Windows.ReprintWindow;

    public class HomeScreenViewModel : BaseViewModel
    {
        public HomeScreenViewModel()
        {
            GV212ButtonVisibility = Visibility.Hidden;
        }

        public double ButtonHeight { get; set; }
        public double ButtonWidth { get; set; }
        public string ColumnWidth { get; set; }

        public double LogoHeight { get; set; }
        public double LogoWidth { get; set; }

        public bool IsGV212OutOfDate { get; set; }
        public Visibility GV212ButtonVisibility { get; set; }
        public bool SmallGV212 { get; set; }

        public DelegateCommand<UserControl> ResizeCommand { get; set; }

        public string VersionNumber
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;
                return string.Format(Resources.TXT_PRODUCT_VERSION, $"{version.Major}.{version.Minor}.{version.Build}");
            }
        }

        public DelegateCommand<object> NewDigitalTachographCommand { get; set; }
        public DelegateCommand<object> NewAnalogueTachographDocumentCommand { get; set; }
        public DelegateCommand<object> NewUndownloadabilityDocumentCommand { get; set; }
        public DelegateCommand<object> GenerateGV212Command { get; set; }
        public DelegateCommand<object> ThreeMonthWalkaroundCommand { get; set; }
        public DelegateCommand<object> QCCheckHistoryCommand { get; set; }
        public DelegateCommand<object> LetterForDecommissioningHistoryCommand { get; set; }
        public DelegateCommand<object> UndownloadabilityHistoryCommand { get; set; }
        public DelegateCommand<object> TachographHistoryCommand { get; set; }
        public DelegateCommand<object> ReprintLabelCommand { get; set; }
        public DelegateCommand<object> ReprintCertificateCommand { get; set; }

        protected override void InitialiseCommands()
        {
            NewDigitalTachographCommand = new DelegateCommand<object>(OnNewDigitalDocument);
            NewAnalogueTachographDocumentCommand = new DelegateCommand<object>(OnNewAnalogueDocument);
            NewUndownloadabilityDocumentCommand = new DelegateCommand<object>(OnNewUndownloadabilityDocument);
            GenerateGV212Command = new DelegateCommand<object>(OnGenerateGV212);
            ResizeCommand = new DelegateCommand<UserControl>(OnResize);
            ThreeMonthWalkaroundCommand = new DelegateCommand<object>(OnThreeMonthWalkaround);
            QCCheckHistoryCommand = new DelegateCommand<object>(OnQCCheckHistory);
            LetterForDecommissioningHistoryCommand = new DelegateCommand<object>(OnLetterForDecommissioningHistory);
            UndownloadabilityHistoryCommand = new DelegateCommand<object>(OnUndownloadabilityHistory);
            TachographHistoryCommand = new DelegateCommand<object>(OnTachographHistory);
            ReprintLabelCommand = new DelegateCommand<object>(OnReprintLabel);
            ReprintCertificateCommand = new DelegateCommand<object>(OnReprintCertificate);
        }

        protected override void Load()
        {
            CheckGV212Status();
        }

        private void OnNewDigitalDocument(object obj)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }

            var viewModel = (NewTachographViewModel) MainWindow.ShowView<NewTachographView>();
            viewModel.SetDocumentTypes(true);
        }
        
        private void OnNewUndownloadabilityDocument(object param)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }
            
            MainWindow.ShowView<NewUndownloadabilityView>();
        }

        private void OnNewAnalogueDocument(object param)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }

            var viewModel = (NewTachographViewModel) MainWindow.ShowView<NewAnalogueTachographView>();
            viewModel.SetDocumentTypes(false);
        }

        private void OnGenerateGV212(object obj)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }
            
            GV212ReportHelper.Create(true);
            CheckGV212Status();
        }

        private void CheckGV212Status()
        {
            if (!HasValidLicense())
            {
                return;
            }

            var settingsRepository = GetInstance<ISettingsRepository<WorkshopSettings>>();
            var workshopSettings = settingsRepository.GetWorkshopSettings();

            if (workshopSettings.IsGV212CheckEnabled && workshopSettings.MonthlyGV212Date != null)
            {
                var statusReport = new StatusReportViewModel();
                if (statusReport.IsGV212Due() && GV212ReportHelper.HasDataForThisMonth())
                {
                    IsGV212OutOfDate = true;
                }
                else
                {
                    IsGV212OutOfDate = false;
                }
                GV212ButtonVisibility = Visibility.Visible;
            }
        }

        private void OnReprintLabel(object obj)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }

            var window = new ReprintWindow
            {
                DataContext = new ReprintWindowViewModel
                {
                    ReprintMode = ReprintMode.Label
                }
            };
            window.ShowDialog();
        }

        private void OnReprintCertificate(object obj)
        {
            if (!HasValidLicense())
            {
                ShowInvalidLicenseWarning();
                return;
            }

            var window = new ReprintWindow
            {
                DataContext = new ReprintWindowViewModel
                {
                    ReprintMode = ReprintMode.Certificate
                }
            };
            window.ShowDialog();
        }

        private void OnResize(UserControl userControl)
        {
            if (userControl == null)
            {
                return;
            }

            if (userControl.ActualHeight < 580)
            {
                ButtonHeight = 224;
                ButtonWidth = 185;
                LogoWidth = 400;
                LogoHeight = 160;
                SmallGV212 = true;
            }
            else
            {
                ButtonHeight = 285;
                ButtonWidth = 235;
                LogoWidth = 625;
                LogoHeight = 250;
                SmallGV212 = false;
            }
        }

        private void OnTachographHistory(object obj)
        {
            MainWindow.TachographHistoryCommand.Execute(null);
        }

        private void OnUndownloadabilityHistory(object obj)
        {
            MainWindow.UndownloadabilityHistoryCommand.Execute(null);
        }

        private void OnLetterForDecommissioningHistory(object obj)
        {
            MainWindow.LetterForDecommissioningHistoryCommand.Execute(null);
        }

        private void OnQCCheckHistory(object obj)
        {
            MainWindow.QCCheckHistoryCommand.Execute(null);
        }

        private void OnThreeMonthWalkaround(object obj)
        {
            MainWindow.QC6MonthCheckCommand.Execute(null);
        }
    }
}