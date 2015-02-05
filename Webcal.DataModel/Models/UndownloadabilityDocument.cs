namespace Webcal.DataModel
{
    using System;

    [Serializable]
    public class UndownloadabilityDocument : Document
    {
        public override bool IsNew
        {
            get { return false; }
        }
    }
}