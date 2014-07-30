using System;

namespace Webcal.DataModel
{
    public class BaseModel : BaseNotification, ICloneable
    {
        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        public virtual T Clone<T>()
        {
            return (T)Clone();
        }
    }
}
