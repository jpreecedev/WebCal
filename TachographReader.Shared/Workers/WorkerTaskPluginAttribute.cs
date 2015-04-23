namespace TachographReader.Shared.Workers
{
    using System;

    public class WorkerTaskPluginAttribute : Attribute
    {
        public WorkerTaskPluginAttribute(Type pluginType)
        {
            PluginType = pluginType;
        }

        public Type PluginType { get; set; }
    }
}