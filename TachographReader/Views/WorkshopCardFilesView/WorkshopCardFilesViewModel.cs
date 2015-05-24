namespace TachographReader.Views
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows.Controls;
    using System.Xml.Linq;
    using Windows.DriverCardDetailsWindow;
    using Windows.ProgressWindow;
    using Windows.WorkshopCardDetailsWindow;
    using Core;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using EventArguments;
    using Library;
    using Properties;
    using Shared;
    using Shared.Helpers;

    public class WorkshopCardFilesViewModel : BaseFilesViewModel
    {
        private ProgressWindow _progressWindow;
        public IRepository<WorkshopCardFile> WorkshopCardFilesRepository { get; set; }
        public string Workshop { get; set; }
        public bool IsReadFromCardEnabled { get; set; }
        public bool IsFormEnabled { get; set; }
        public DelegateCommand<Grid> ReadFromCardCommand { get; set; }
        public IDriverCardReader DriverCardReader { get; set; }

        protected override void Load()
        {
            IsReadFromCardEnabled = true;
            IsFormEnabled = true;
            StoredFiles.AddRange(WorkshopCardFilesRepository.GetAll());

            DriverCardReader = new DriverCardReader();
            DriverCardReader.Completed += Completed;
            DriverCardReader.Progress += Progress;
        }

        protected override void InitialiseRepositories()
        {
            WorkshopCardFilesRepository = GetInstance<IRepository<WorkshopCardFile>>();
        }

        protected override void InitialiseCommands()
        {
            base.InitialiseCommands();

            ReadFromCardCommand = new DelegateCommand<Grid>(OnReadFromCard);
        }

        protected override void OnAddStoredFile()
        {
            if (!File.Exists(FilePath))
            {
                ShowError(Resources.MSG_EXC_CANNOT_ACCESS_FILE);
                return;
            }

            try
            {
                WorkshopCardFile workshopCardFile = WorkshopCardFile.GetWorkshopCardFile(SelectedDate, Workshop, FilePath);
                StoredFiles.Add(workshopCardFile);
                WorkshopCardFilesRepository.Add(workshopCardFile.Clone<WorkshopCardFile>());

                IsReadFromCardEnabled = true;
                IsFormEnabled = true;
                Workshop = null;
                SelectedDate = DateTime.Now;
                FilePath = null;
            }
            catch (Exception ex)
            {
                ShowError(Resources.EXC_UNABLE_ADD_WORKSHOP_CARD_FILE, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex));
            }
        }

        protected override void OnShowFileDetails()
        {
            var window = new WorkshopCardDetailsWindow();
            ((WorkshopCardDetailsViewModel)window.DataContext).WorkshopCardFile = SelectedStoredFile;
            window.ShowDialog();
        }

        protected override void OnStoredFileRemoved()
        {
            WorkshopCardFilesRepository.Remove((WorkshopCardFile) SelectedStoredFile);
        }

        protected override void OnEmptyFields()
        {
            IsReadFromCardEnabled = true;
            IsFormEnabled = true;
        }

        public override void OnClosing(bool cancelled)
        {
            DriverCardReader.Dispose();
        }

        private void OnReadFromCard(Grid root)
        {
            if (root == null)
            {
                return;
            }

            DriverCardReader.GenerateDump();

            _progressWindow = new ProgressWindow();
            _progressWindow.ShowDialog();
        }

        private void Progress(object sender, DriverCardProgressEventArgs e)
        {
            ((ProgressWindowViewModel) _progressWindow.DataContext).ProgressText = e.Message;
        }

        private void Completed(object sender, DriverCardCompletedEventArgs e)
        {
            _progressWindow.Close();

            if (!e.IsSuccess)
            {
                ShowError(Resources.TXT_UNABLE_READ_SMART_CARD);
                return;
            }

            DisplayWorkshopCardDetails(e.DumpFilePath);
        }

        private void DisplayWorkshopCardDetails(string xml)
        {
            try
            {
                SelectedDate = DateTime.Now;
                XDocument document = XDocument.Parse(xml);
                XElement first = document.Descendants("CardDump").FirstOrDefault();

                if (first != null)
                {
                    FilePath = first.Element("TempPath").SafelyGetValue();
                    Workshop = first.Element("WorkshopName").SafelyGetValue();
                    IsReadFromCardEnabled = false;
                    IsFormEnabled = false;
                }

                if (!File.Exists(FilePath))
                {
                    FilePath = null;
                    Workshop = null;
                    IsReadFromCardEnabled = true;
                    IsFormEnabled = true;

                    MessageBoxHelper.ShowError(Resources.ERR_UNABLE_GENERATE_CARD_DUMP);
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError(string.Format("{0}\n\n{1}", Resources.TXT_UNABLE_READ_SMART_CARD, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex)));
            }
        }
    }
}