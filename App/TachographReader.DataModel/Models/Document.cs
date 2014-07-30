using System;

namespace Webcal.DataModel
{
    public abstract class Document : BaseModel
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public string DocumentType { get; set; }

        public string Office { get; set; }

        public string RegistrationNumber { get; set; }

        public string TachographMake { get; set; }

        public string TachographModel { get; set; }

        public string SerialNumber { get; set; }

        public DateTime? InspectionDate { get; set; }

        public string Technician { get; set; }

        public string CustomerContact { get; set; }
    }
}
