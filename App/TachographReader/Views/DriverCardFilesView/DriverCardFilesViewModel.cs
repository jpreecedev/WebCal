using System;
using System.IO;
using StructureMap;
using Webcal.Core;
using Webcal.DataModel;
using Webcal.Library;
using Webcal.Shared;
using Webcal.Properties;

namespace Webcal.Views
{
    public class DriverCardFilesViewModel : BaseFilesViewModel
    {
        #region Public Properties

        public IRepository<DriverCardFile> DriverCardFilesRepository { get; set; }

        public string Driver { get; set; }

        #endregion

        #region Overrides

        protected override void Load()
        {
            StoredFiles.AddRange(DriverCardFilesRepository.GetAll());
        }

        protected override void InitialiseRepositories()
        {
            DriverCardFilesRepository = ObjectFactory.GetInstance<IRepository<DriverCardFile>>();
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
                DriverCardFile driverCardFile = new DriverCardFile
                                                    {
                                                        Date = SelectedDate,
                                                        Customer = SelectedCustomerContact.Clone<CustomerContact>(),
                                                        Driver = Driver,
                                                        FileName = Path.GetFileName(FilePath),
                                                        SerializedFile = BaseFile.GetStoredFile(FilePath)
                                                    };

                StoredFiles.Add(driverCardFile);
                DriverCardFilesRepository.Add(driverCardFile);
            }
            catch (Exception ex)
            {
                ShowError(Resources.EXC_UNABLE_TO_CREATE_DRIVER_CARD, ExceptionPolicy.HandleException(ex));
            }
        }

        protected override void OnStoredFileRemoved()
        {
            DriverCardFilesRepository.Remove((DriverCardFile)SelectedStoredFile);
        }

        public override void OnClosing(bool cancelled)
        {
            DriverCardFilesRepository.Save();
        }

        #endregion
    }
}
