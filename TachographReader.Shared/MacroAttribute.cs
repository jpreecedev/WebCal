namespace Webcal.Shared
{
    using System;

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class MacroAttribute : Attribute
    {
        public string Placeholder { get; set; }

        public string Meaning { get; set; }
    }
}