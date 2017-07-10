namespace TachographReader.Views
{
    using System;
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

    public class HomeScreenViewModel : BaseViewModel
    {
        public HomeScreenViewModel()
        {
            GV212ButtonVisibility = Visibility.Hidden;
        }

        public bool IsGV212OutOfDate { get; set; }
        public Visibility GV212ButtonVisibility { get; set; }
        public bool IsCondensed { get; set; }

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

        private void OnResize(UserControl userControl)
        {
            if (userControl == null)
            {
                return;
            }

            IsCondensed = userControl.ActualHeight < 650;
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