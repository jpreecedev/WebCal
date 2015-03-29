namespace TachographReader.Shared.Models
{
    using System;

    [Serializable]
    public class DiscreteException
    {
        public string ExceptionDetails { get; set; }
        public string Occurred { get; set; }
    }
}