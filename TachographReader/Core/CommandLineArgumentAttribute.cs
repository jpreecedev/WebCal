namespace TachographReader.Core
{
    using System;

    public class CommandLineArgumentAttribute : Attribute
    {
        public string Value { get; set; }

        public CommandLineArgumentAttribute(string value)
        {
            Value = value;
        }
    }
}