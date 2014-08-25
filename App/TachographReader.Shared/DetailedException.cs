namespace Webcal.Shared
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    [DataContract, Serializable]
    public class DetailedException
    {
        [DataMember]
        public int Id { get; set; }

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