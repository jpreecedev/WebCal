namespace Webcal.DataModel
{
    using System.Collections.ObjectModel;
    using Properties;

    public static class DocumentType
    {
        public static ObservableCollection<string> GetDocumentTypes(bool isDigital)
        {
            var documentTypes = new ObservableCollection<string>();

            if (isDigital)
            {
                documentTypes.Add(Resources.TXT_DIGITAL_TWO_YEAR_CALIBRATION);
                documentTypes.Add(Resources.TXT_DIGITAL_INITIAL_CALIBRATION);
            }
            else
            {
                documentTypes.Add(Resources.TXT_TWO_YEAR_INSPECTION);
                documentTypes.Add(Resources.TXT_INSTALLATION_CALIBRATION);
                documentTypes.Add(Resources.TXT_RECALIBRATION);
                documentTypes.Add(Resources.TXT_SIX_YEAR_CALIBRATION);
            }

            documentTypes.Add(Resources.TXT_MINOR_WORK_DETAILS);
            
            return documentTypes;
        }
    }
}