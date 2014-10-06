namespace Webcal.EmailWorker
{
    using Shared.Workers;

    public class EmailParameters : WorkerParameters
    {
        public EmailParameters(IWorkerParameters parameters)
            :base(parameters)
        {
            
        }
        
        public bool PersonaliseMyEmails
        {
            get { return GetParameter<bool>("PersonaliseMyEmails"); }
        }

        public string AttachmentPath
        {
            get { return GetParameter<string>("AttachmentPath"); }
        }

        public string Recipient
        {
            get { return GetParameter<string>("Recipient"); }
        }

        public string Subject
        {
            get { return GetParameter<string>("Subject"); }
        }

        public string Body
        {
            get { return GetParameter<string>("Body"); }
        }
    }
}