namespace Webcal.Shared.Workers
{
    using System;
    using System.Configuration;

    public class BaseElementCollection<T> : ConfigurationElementCollection where T : ConfigurationElement, new()
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }

        public void Add(T customElement, string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                BaseAdd(customElement);
            }
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            throw new NotImplementedException();
        }
    }
}