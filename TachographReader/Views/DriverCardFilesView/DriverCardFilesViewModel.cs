namespace TachographReader.Views
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Windows.DriverCardDetailsWindow;
    using Core;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using EventArguments;
    using Library;
    using Properties;
    using Shared;
    using Shared.Helpers;

    public class DriverCardFilesViewModel : BaseFilesViewModel
    {
        public IRepository<DriverCardFile> DriverCardFilesRepository { get; set; }
        public string Driver { get; set; }

        protected override void Load()
        {
            base.Load();

            StoredFiles.AddRange(DriverCardFilesRepository.GetAll("Customer").OrderByDescending(c => c.Date));
        }

        protected override void InitialiseRepositories()
        {
            DriverCardFilesRepository = GetInstance<IRepository<DriverCardFile>>();
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
                var driverCardFile = new DriverCardFile
                {
                    Date = SelectedDate,
                    Driver = Driver,
                    FileName = Path.GetFileName(FilePath),
                    SerializedFile = BaseFile.GetStoredFile(FilePath)
                };

                StoredFiles.Add(driverCardFile);
                DriverCardFilesRepository.Add(driverCardFile);

                Driver = null;
                SelectedDate = DateTime.Now;
                FilePath = null;
            }
            catch (Exception ex)
            {
                ShowError(Resources.EXC_UNABLE_TO_CREATE_DRIVER_CARD, ExceptionPolicy.HandleException(ContainerBootstrapper.Container, ex));
            }
        }

        protected override void OnShowFileDetails()
        {
            var window = new DriverCardDetailsWindow();
            ((DriverCardDetailsViewModel)window.DataContext).DriverCardFile = SelectedStoredFile;
            window.ShowDialog();
        }

        protected override void OnStoredFileRemoved()
        {
            DriverCardFilesRepository.Remove((DriverCardFile) SelectedStoredFile);
        }

        protected override void OnReadComplete(string dumpFilePath)
        {
            DisplayDriverCardDetails(dumpFilePath);
        }

        private void DisplayDriverCardDetails(string xml)
        {
            try
            {
                SelectedDate = DateTime.Now;
                XDocument document = XDocument.Parse(xml);
                XElement first = document.Descendants("CardDump").FirstOrDefault();

                if (first != null)
                {
                    FilePath = first.Element("TempPath").SafelyGetValue();
                    Driver = null;
                    IsReadFromCardEnabled = false;
                    IsFormEnabled = false;
                }

                if (!File.Exists(FilePath))
                {
                    FilePath = null;
                    Driver = null;
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