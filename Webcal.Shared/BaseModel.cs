namespace Webcal.Shared
{
    using System;
    using System.Runtime.Serialization;

    public class BaseModel : BaseNotification, ICloneable
    {
        [DataMember]
        public int Id { get; set; }

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