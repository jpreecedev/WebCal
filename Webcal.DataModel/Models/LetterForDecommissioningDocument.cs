namespace Webcal.DataModel
{
    using System;

    [Serializable]
    public class LetterForDecommissioningDocument : Document
    {
        public override bool IsNew
        {
            get { return false; }
        }
    }
}