﻿namespace TachographReader.Views
{
    using System;
    using System.IO;
    using System.Linq;
    using Windows.CalibrationDetailsWindow;
    using Windows.DriverCardDetailsWindow;
    using Connect.Shared.Models;
    using Core;
    using DataModel;
    using DataModel.Core;
    using Library;
    using Properties;
    using Shared;

    public class DriverCardFilesViewModel : BaseFilesViewModel
    {
        public IRepository<DriverCardFile> DriverCardFilesRepository { get; set; }
        public string Driver { get; set; }

        protected override void Load()
        {
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
    }
}