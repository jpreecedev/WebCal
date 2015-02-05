namespace Webcal.Shared.Core
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    [Serializable]
    public class BaseModel : BaseNotification, ICloneable
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public DateTime? Deleted { get; set; }

        [XmlIgnore]
        public bool IsDeleted
        {
            get { return Deleted != null; }
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        public virtual T Clone<T>()
        {
            return (T) Clone();
        }
    }
}