using System;

namespace Webcal.Shared
{
    [Serializable]
    public class DiscreteException
    {
        public string ExceptionDetails { get; set; }

        public string Occurred { get; set; }
    }
}