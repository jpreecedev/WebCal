namespace Webcal.Library
{
    using System.Linq;
    using Connect.Shared.Models;
    using Core;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using Shared;
    using Shared.Helpers;
    using Shared.Workers;

    public static class LabelHelper
    {
        public static void Print(TachographDocument document)
        {
            var workerTask = new WorkerTask {TaskName = WorkerTaskName.LabelPrint};

            var registrationData = GetRegistrationData();
            var workshopSettings = GetWorkshopSettings();
            var printerSettings = GetPrinterSettings();

            workerTask.Parameters = new WorkerParameters();
            workerTask.Parameters.SetParameter("AutoPrintLabels", printerSettings.AutoPrintLabels);
            workerTask.Parameters.SetParameter("LabelNumberOfCopies", printerSettings.LabelNumberOfCopies);

            workerTask.Parameters.SetParameter("Address1", workshopSettings.Address1);
            workerTask.Parameters.SetParameter("Town", workshopSettings.Town);
            workerTask.Parameters.SetParameter("PostCode", workshopSettings.PostCode);
            workerTask.Parameters.SetParameter("WorkshopName", workshopSettings.WorkshopName);
            workerTask.Parameters.SetParameter("PhoneNumber", workshopSettings.PhoneNumber);
            workerTask.Parameters.SetParameter("DateFormat", Constants.DateFormat);
            workerTask.Parameters.SetParameter("LicenseKey", registrationData.LicenseKey);
            workerTask.Parameters.SetParameter("ExpirationDate", registrationData.ExpirationDate);
            workerTask.Parameters.SetParameter("DefaultLabelPrinter", printerSettings.DefaultLabelPrinter);
            workerTask.Parameters.SetParameter("CompanyName", registrationData.CompanyName);
            workerTask.Parameters.SetParameter("TemporaryDirectory", ImageHelper.GetTemporaryDirectory());

            workerTask.Parameters.SetParameter("DocumentType", document.DocumentType);
            workerTask.Parameters.SetParameter("KFactor", document.KFactor);
            workerTask.Parameters.SetParameter("WFactor", document.WFactor);
            workerTask.Parameters.SetParameter("LFactor", document.LFactor);
            workerTask.Parameters.SetParameter("VIN", document.VIN);
            workerTask.Parameters.SetParameter("SerialNumber", document.SerialNumber);
            workerTask.Parameters.SetParameter("TyreSize", document.TyreSize);
            workerTask.Parameters.SetParameter("CalibrationTime", document.CalibrationTime);

            workerTask.Parameters.SetParameter("SealNumber", registrationData.SealNumber);
            workerTask.Parameters.SetParameter("SkillrayTachoIcon", DocumentHelper.GetResourceFromSimplePath("Images/PDF/skillray-tacho-icon.png"));

            WorkerHelper.RunTask(workerTask);
        }

        private static RegistrationData GetRegistrationData()
        {
            return ContainerBootstrapper.Container.GetInstance<IRepository<RegistrationData>>().GetAll().First();
        }

        private static WorkshopSettings GetWorkshopSettings()
        {
            return ContainerBootstrapper.Container.GetInstance<ISettingsRepository<WorkshopSettings>>().GetWorkshopSettings();
        }

        private static PrinterSettings GetPrinterSettings()
        {
            return ContainerBootstrapper.Container.GetInstance<ISettingsRepository<PrinterSettings>>().GetPrinterSettings();
        }
    }
}