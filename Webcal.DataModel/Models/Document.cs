namespace Webcal.DataModel
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using Shared.Core;

    [Serializable]
    [KnownType(typeof(TachographDocument))]
    [KnownType(typeof(UndownloadabilityDocument))]
    [KnownType(typeof(LetterForDecommissioningDocument))]
    public abstract class Document : BaseModel
    {
        private string _documentType;
        public DateTime Created { get; set; }

        public string DocumentType
        {
            get { return _documentType; }
            set
            {
                _documentType = value;
                OnDocumentTypeChanged(value);
            }
        }

        public string Office { get; set; }
        public string RegistrationNumber { get; set; }
        public string TachographMake { get; set; }
        public string TachographModel { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? InspectionDate { get; set; }
        public string Technician { get; set; }
        public string CustomerContact { get; set; }
       
        [XmlIgnore]
        public abstract bool IsNew { get; }

        protected virtual void OnDocumentTypeChanged(string newValue)
        {
        }
    }
}