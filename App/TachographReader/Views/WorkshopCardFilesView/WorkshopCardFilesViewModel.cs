using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;
using StructureMap;
using Webcal.Core;
using Webcal.DataModel.Library;
using Webcal.DataModel;
using Webcal.Library;
using Webcal.Shared;
using Webcal.Windows.ProgressWindow;
using Webcal.Properties;

namespace Webcal.Views
{
    public class WorkshopCardFilesViewModel : BaseFilesViewModel
    {
        #region Public Properties

        public IRepository<WorkshopCardFile> WorkshopCardFilesRepository { get; set; }

        public string Workshop { get; set; }

        public bool IsReadFromCardEnabled { get; set; }

        public bool IsFormEnabled { get; set; }

        #endregion

        #region Overrides

        protected override void Load()
        {
            IsReadFromCardEnabled = true;
            IsFormEnabled = true;
            StoredFiles.AddRange(WorkshopCardFilesRepository.GetAll());
        }

        protected override void InitialiseRepositories()
        {
            WorkshopCardFilesRepository = ObjectFactory.GetInstance<IRepository<WorkshopCardFile>>();
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
                ShowError(Resources.EXC_UNABLE_ADD_WORKSHOP_CARD_FILE, ExceptionPolicy.HandleException(ex));
            }
        }

        protected override void OnStoredFileRemoved()
        {
            WorkshopCardFilesRepository.Remove((WorkshopCardFile)SelectedStoredFile);
        }

        protected override void OnEmptyFields()
        {
            IsReadFromCardEnabled = true;
            IsFormEnabled = true;
        }

        public override void OnClosing(bool cancelled)
        {
            WorkshopCardFilesRepository.Save();
        }

        #endregion

        #region Commands

        #region Command : Read From Card

        public DelegateCommand<Grid> ReadFromCardCommand { get; set; }

        private void OnReadFromCard(Grid root)
        {
            if (root == null)
                return;

            ProgressWindow progressWindow = new ProgressWindow();

            Task<string> task = new Task<string>(SmartCardMonitor.Instance.GetCardDump);
            task.Start();
            task.ContinueWith(t =>
                                  {
                                      progressWindow.Close();

                                      if (string.IsNullOrEmpty(t.Result))
                                      {
                                          ShowError(Resources.TXT_UNABLE_READ_SMART_CARD);
                                          return;
                                      }

                                      DisplayWorkshopCardDetails(t.Result);
                                  },
                              TaskScheduler.FromCurrentSynchronizationContext());

            progressWindow.ShowDialog();
        }

        #endregion

        #endregion

        #region Private Methods

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

                    MessageBoxHelper.ShowError("Unable to generate card dump");
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError(string.Format("{0}\n\n{1}", Resources.TXT_UNABLE_READ_SMART_CARD, ExceptionPolicy.HandleException(ex)));
            }
        }

        #endregion
    }
}