namespace TachographReader.Library
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Connect.Shared.Models;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using Properties;
    using Shared;

    public static class TachographDocumentHelper
    {
        public static TachographDocument Create(CalibrationRecord calibrationRecord)
        {
            var tachographDocument = new TachographDocument();
            tachographDocument.Convert(calibrationRecord);
            tachographDocument.IsDigital = true;

            var documentTypes = DocumentType.GetDocumentTypes(true);
            var settings = ContainerBootstrapper.Resolve<ISettingsRepository<MiscellaneousSettings>>().GetMiscellaneousSettings();
            tachographDocument.DocumentType = tachographDocument.DocumentType ?? documentTypes.FirstOrDefault(c => string.Equals(c, settings.DefaultDigitalDocumentType));

            SetTechnician(tachographDocument, calibrationRecord);
            SetTachographModel(tachographDocument, calibrationRecord);
            SetDepotName(tachographDocument);

            tachographDocument.Created = DateTime.Now;

            if (tachographDocument.CalibrationTime == null)
            {
                tachographDocument.CalibrationTime = DateTime.Now;
            }

            return tachographDocument;
        }

        private static void SetDepotName(Document document)
        {
            var registrationData = ContainerBootstrapper.Resolve<IRepository<RegistrationData>>().First();
            if (string.IsNullOrEmpty(document.DepotName))
            {
                document.DepotName = registrationData.DepotName;
            }
        }

        private static void SetTachographModel(Document document, CalibrationRecord calibrationRecord)
        {
            if (!string.IsNullOrEmpty(document.TachographMake) && !string.IsNullOrEmpty(calibrationRecord.VuPartNumber))
            {
                if (string.Equals(document.TachographMake, Resources.TXT_SIEMENS_VDO, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (calibrationRecord.VuPartNumber.StartsWith(DataModel.Properties.Resources.TXT_SEED_TACHO_MODEL_NAME))
                    {
                        document.TachographModel = calibrationRecord.VuPartNumber;
                    }
                }
                if (string.Equals(document.TachographMake, Resources.TXT_STONERIDGE, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (calibrationRecord.VuPartNumber.StartsWith(Resources.TXT_STONERIDGE_CARD))
                    {
                        document.TachographModel = calibrationRecord.VuPartNumber;
                    }
                }
            }
        }

        private static void SetTechnician(Document document, CalibrationRecord calibrationRecord)
        {
            var techniciansRepository = ContainerBootstrapper.Resolve<IRepository<Technician>>();
            var technicians = new ObservableCollection<Technician>(techniciansRepository.GetAll());

            if (!technicians.IsNullOrEmpty() && !string.IsNullOrEmpty(calibrationRecord.CardSerialNumber))
            {
                foreach (var technician in technicians)
                {
                    if (technician != null && !string.IsNullOrEmpty(technician.Number))
                    {
                        if (string.Equals(technician.Number, calibrationRecord.CardSerialNumber))
                        {
                            document.Technician = technician.Name;
                        }
                    }
                }
            }
        }
    }
}