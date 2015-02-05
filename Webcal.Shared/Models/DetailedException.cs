namespace Webcal.Shared.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using Webcal.Connect.Shared.Models;

    [DataContract, Serializable]
    public class DetailedException : BaseModel
    {
        [DataMember]
        [MaxLength]
        public string ExceptionDetails { get; set; }

        [DataMember]
        public string ApplicationName { get; set; }

        [DataMember]
        [MaxLength]
        public byte[] RawImage { get; set; }

        [DataMember]
        public DateTime Occurred { get; set; }
    }
}