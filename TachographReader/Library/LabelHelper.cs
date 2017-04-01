namespace TachographReader.Library
{
    using System.Linq;
    using Connect.Shared;
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
            var registrationData = GetRegistrationData();
            var workshopSettings = GetWorkshopSettings();
            var printerSettings = GetPrinterSettings();

            var parameters = new WorkerParameters();
            parameters.SetParameter("AutoPrintLabels", printerSettings.AutoPrintLabels);
            parameters.SetParameter("LabelNumberOfCopies", printerSettings.LabelNumberOfCopies);
            
            parameters.SetParameter("Address1", workshopSettings.Address1);
            parameters.SetParameter("Town", workshopSettings.Town);
            parameters.SetParameter("PostCode", workshopSettings.PostCode);
            parameters.SetParameter("WorkshopName", workshopSettings.WorkshopName);
            parameters.SetParameter("PhoneNumber", workshopSettings.PhoneNumber);
            parameters.SetParameter("DateFormat", Constants.LongYearDateFormat);
            parameters.SetParameter("LicenseKey", registrationData.LicenseKey);
            parameters.SetParameter("ExpirationDate", registrationData.ExpirationDate);
            parameters.SetParameter("DefaultLabelPrinter", printerSettings.DefaultLabelPrinter);
            parameters.SetParameter("CompanyName", registrationData.CompanyName);
            parameters.SetParameter("TemporaryDirectory", ImageHelper.GetTemporaryDirectory());
            parameters.SetParameter("DefaultFont", printerSettings.DefaultFont);
            parameters.SetParameter("ShowCompanyNameOnLabels", printerSettings.ShowCompanyNameOnLabels);
            
            parameters.SetParameter("DocumentType", document.DocumentType);
            parameters.SetParameter("KFactor", document.KFactor);
            parameters.SetParameter("WFactor", document.WFactor);
            parameters.SetParameter("LFactor", document.LFactor);
            parameters.SetParameter("VIN", document.VIN);
            parameters.SetParameter("SerialNumber", document.SerialNumber);
            parameters.SetParameter("TyreSize", document.TyreSize);
            parameters.SetParameter("CalibrationTime", document.CalibrationTime);
            
            parameters.SetParameter("SealNumber", registrationData.SealNumber);
            parameters.SetParameter("SkillrayTachoIcon", ImageHelper.LoadFromResources("skillray_tacho_icon").ToByteArray());

            var workerTask = new WorkerTask {TaskName = WorkerTaskName.LabelPrint};
            workerTask.SetWorkerParameters(parameters);

            WorkerHelper.QueueTask(workerTask);
        }

        private static RegistrationData GetRegistrationData()
        {
            return ContainerBootstrapper.Resolve<IRepository<RegistrationData>>().GetAll().First();
        }

        private static WorkshopSettings GetWorkshopSettings()
        {
            return ContainerBootstrapper.Resolve<ISettingsRepository<WorkshopSettings>>().GetWorkshopSettings();
        }

        private static PrinterSettings GetPrinterSettings()
        {
            return ContainerBootstrapper.Resolve<ISettingsRepository<PrinterSettings>>().GetPrinterSettings();
        }
    }
}