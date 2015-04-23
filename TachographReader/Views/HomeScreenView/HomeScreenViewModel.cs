namespace TachographReader.Views
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

            if (userControl.ActualHeight < 580)
            {
                ButtonHeight = 224;
                ButtonWidth = 185;
            }
            else
            {
                ButtonHeight = 285;
                ButtonWidth = 235;
            }
        }
    }
}