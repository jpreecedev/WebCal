﻿namespace TachographReader.Views
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
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
        public IRepository<WorkshopCardFile> WorkshopCardFilesRepository { get; set; }
        public string Workshop { get; set; }        

        protected override void Load()
        {
            base.Load();

            StoredFiles.AddRange(WorkshopCardFilesRepository.GetAll().OrderByDescending(c => c.Date));
        }

        protected override void InitialiseRepositories()
        {
            WorkshopCardFilesRepository = GetInstance<IRepository<WorkshopCardFile>>();
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

        public override void OnClosing(bool cancelled)
        {
            DriverCardReader.Dispose();
        }

        protected override void OnReadComplete(string dumpFilePath)
        {
            DisplayWorkshopCardDetails(dumpFilePath);            
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