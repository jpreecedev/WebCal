namespace TachographReader.Views
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using Windows.ReprintWindow;
    using Connect.Shared;
    using Core;
    using DataModel.Library;
    using Library;
    using Library.ViewModels;
    using Properties;
    using Shared;

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
        public DelegateCommand<object> ReprintLabelCommand { get; set; }
        public DelegateCommand<object> ReprintCertificateCommand { get; set; }
        public DelegateCommand<UserControl> ResizeCommand { get; set; }
        public DelegateCommand<object> GenerateGV212Command { get; set; }

        protected override void InitialiseCommands()
        {
            NewDigitalTachographCommand = new DelegateCommand<object>(OnNewDigitalDocument);
            NewAnalogueTachographDocumentCommand = new DelegateCommand<object>(OnNewAnalogueDocument);
            ReprintLabelCommand = new DelegateCommand<object>(OnReprintLabel);
            ReprintCertificateCommand = new DelegateCommand<object>(OnReprintCertificate);
            ResizeCommand = new DelegateCommand<UserControl>(OnResize);
            GenerateGV212Command = new DelegateCommand<object>(OnGenerateGV212);
        }

        protected override void Load()
        {
            CheckGV212Status();
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

        private void OnGenerateGV212(object obj)
        {
            if (!IsGV212OutOfDate)
            {
                return;
            }

            if (ShowWarning(Resources.TXT_GV_212_OUT_OF_DATE, Resources.TXT_OUT_OF_DATE_TITLE, MessageBoxButton.YesNo))
            {
                var settingsRepository = GetInstance<ISettingsRepository<WorkshopSettings>>();
                var workshopSettings = settingsRepository.GetWorkshopSettings();

                var start = DateTime.Parse("01/" + DateTime.Now.Month + "/" + DateTime.Now.Year).AddMonths(-1);
                var end = start.AddMonths(1).AddDays(-1);

                GV212ReportHelper.Create(start, end);
                workshopSettings.MonthlyGV212Date = DateTime.Now.Date;
                settingsRepository.Save(workshopSettings);

                CheckGV212Status();
            }
        }

        private void CheckGV212Status()
        {
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
    }
}