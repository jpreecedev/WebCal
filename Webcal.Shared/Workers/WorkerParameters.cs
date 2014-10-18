namespace Webcal.Shared.Workers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class WorkerParameters : IWorkerParameters
    {
        private IDictionary<string, object> _parameters;

        public WorkerParameters()
        {
            _parameters = new Dictionary<string, object>();
        }

        public WorkerParameters(IWorkerParameters parameters)
        {
            _parameters = parameters.GetParameters();
        }

        public T GetParameter<T>(string key)
        {
            return (T) _parameters[key];
        }

        public IDictionary<string, object> GetParameters()
        {
            return _parameters;
        }

        public void SetParameter<T>(string key, T value)
        {
            _parameters.Add(key, value);
        }

        public string Serialize()
        {
            if (_parameters == null || _parameters.Count == 0)
            {
                return string.Empty;
            }

            var serializer = new XmlSerializer(typeof (List<SerializableKeyValuePair<string, object>>));

            var settings = new XmlWriterSettings
            {
                Encoding = new UnicodeEncoding(false, false),
                Indent = false,
                OmitXmlDeclaration = false
            };

            using (var textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, ToSerializableKeyValuePair());
                }
                return textWriter.ToString();
            }
        }

        public void Deserialize(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                return;
            }

            var serializer = new XmlSerializer(typeof (List<SerializableKeyValuePair<string, object>>));

            using (var textReader = new StringReader(parameters))
            {
                using (XmlReader xmlReader = XmlReader.Create(textReader, new XmlReaderSettings()))
                {
                    List<SerializableKeyValuePair<string, object>> deserialized = (List<SerializableKeyValuePair<string, object>>) serializer.Deserialize(xmlReader);
                    _parameters = ToDictionary(deserialized);
                }
            }
        }

        private List<SerializableKeyValuePair<string, object>> ToSerializableKeyValuePair()
        {
            return _parameters.Select(parameter => new SerializableKeyValuePair<string, object> {Key = parameter.Key, Value = parameter.Value}).ToList();
        }

        private static IDictionary<string, object> ToDictionary(IEnumerable<SerializableKeyValuePair<string, object>> value)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();

            foreach (var keyValuePair in value)
            {
                result.Add(keyValuePair.Key, keyValuePair.Value);
            }

            return result;
        }
    }
}