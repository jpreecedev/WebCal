namespace Webcal.DataModel
{
    using System.Collections.Generic;

    public class WorkshopSettings : BaseModel
    {
        private bool _doNotSend;
        private bool _sendToOffice;
        private bool _sendToCustomer;

        public int Id { get; set; }

        public bool AutoBackup { get; set; }

        public string BackupFilePath { get; set; }

        public IList<CustomDayOfWeek> BackupDaysOfWeek { get; set; }

        public string Office { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Town { get; set; }

        public string PostCode { get; set; }

        public string WorkshopName { get; set; }

        public string PhoneNumber { get; set; }

        public bool AutoPrintLabels { get; set; }

        public string MainEmailAddress { get; set; }

        public string SecondaryEmailAddress { get; set; }

        public bool SendToCustomer
        {
            get { return _sendToCustomer; }
            set
            {
                _sendToCustomer = value;
                if (_sendToCustomer)
                {
                    DoNotSend = false;
                }
            }
        }

        public bool SendToOffice
        {
            get { return _sendToOffice; }
            set
            {
                _sendToOffice = value;
                if (_sendToOffice)
                {
                    DoNotSend = false;
                }
            }
        }

        public bool DoNotSend
        {
            get { return _doNotSend; }
            set
            {
                _doNotSend = value;

                if (_doNotSend)
                {
                    SendToOffice = false;
                    SendToCustomer = false;
                }
            }
        }
    }
}