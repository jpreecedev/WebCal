namespace Webcal.Core
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal sealed class BaseControlAttribute : Attribute
    {
    }
}