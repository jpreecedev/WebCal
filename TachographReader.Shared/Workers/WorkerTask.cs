namespace TachographReader.Shared.Workers
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using global::Connect.Shared.Models;

    public class WorkerTask : BaseModel, IWorkerTask
    {
        private string _rawParametersAsXML;
        private IWorkerParameters _parameters;
        
        [MaxLength]
        public string ParametersAsString
        {
            get
            {
                if (_parameters == null)
                {
                    return string.Empty;
                }

                return _parameters.Serialize();
            }
            set
            {
                _rawParametersAsXML = value;
            }
        }

        public WorkerTaskName TaskName { get; set; }
        
        public DateTime Added { get; set; }
        public bool IsProcessing { get; set; }
        public DateTime? Processed { get; set; }
        public Guid WorkerId { get; set; }
        public string Message { get; set; }

        public IWorkerParameters GetWorkerParameters()
        {
            if (!string.IsNullOrEmpty(_rawParametersAsXML))
            {
                IWorkerParameters result = new WorkerParameters();
                result.Deserialize(_rawParametersAsXML);
                return result;
            }
            return null;
        }

        public void SetWorkerParameters(IWorkerParameters parameters)
        {
            _parameters = parameters;
        }
    }
}