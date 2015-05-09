namespace TachographReader.Shared.Workers
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;
    using global::Connect.Shared.Models;

    public class WorkerTask : BaseModel, IWorkerTask
    {
        [MaxLength]
        public string ParametersAsString
        {
            get
            {
                if (Parameters == null)
                {
                    return string.Empty;
                }

                return Parameters.Serialize();
            }
            set
            {
                Parameters = new WorkerParameters();
                Parameters.Deserialize(value);
            }
        }

        public WorkerTaskName TaskName { get; set; }

        [XmlIgnore]
        public IWorkerParameters Parameters { get; set; }

        public DateTime Added { get; set; }
        public bool IsProcessing { get; set; }
        public DateTime? Processed { get; set; }
        public Guid WorkerId { get; set; }
        public string Message { get; set; }
    }
}