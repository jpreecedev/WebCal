﻿namespace TachographReader.Views
{
    using System.Reflection;
    using System.Windows.Controls;
    using Windows.ReprintWindow;
    using Core;
    using Library;
    using Properties;

    public class HomeScreenViewModel : BaseViewModel
    {
        public double ButtonHeight { get; set; }
        public double ButtonWidth { get; set; }
        public string DigitalText { get; set; }
        public string AnalogueText { get; set; }
        public string CertificateText { get; set; }
        public string PlaqueText { get; set; }
        public string ColumnWidth { get; set; }

        public string VersionNumber
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                return string.Format(Resources.TXT_PRODUCT_VERSION, assembly.GetName().Version);
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
                ButtonHeight = 285;
                ButtonWidth = 235;

                DigitalText = Resources.TXT_DIGITAL_TACHOGRAPH_DOCUMENT;
                AnalogueText = Resources.TXT_ANALOGUE_TACHOGRAPH_DOCUMENT;
                CertificateText = Resources.TXT_CERTIFICATE_TACHOGRAPH_DOCUMENT;
                PlaqueText = Resources.TXT_PLAQUE_TACHOGRAPH_DOCUMENT;
            }
        }
    }
}