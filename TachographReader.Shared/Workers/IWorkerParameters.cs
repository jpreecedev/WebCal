namespace TachographReader.Shared.Workers
{
    using System.Collections.Generic;

    public interface IWorkerParameters
    {
        IDictionary<string, object> GetParameters();
        T GetParameter<T>(string key);
        void SetParameter<T>(string key, T value);
        string Serialize();
        void Deserialize(string parameters);
    }
}