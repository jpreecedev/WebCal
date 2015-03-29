namespace TachographReader.Shared
{
    using System;
    using System.Xml.Serialization;

    [Serializable]
    [XmlType(TypeName = "SerializableKeyValuePair")]
    public struct SerializableKeyValuePair<T, T2>
    {
        public T Key { get; set; }
        public T2 Value { get; set; }
    }
}