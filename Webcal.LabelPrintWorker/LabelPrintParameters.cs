namespace Webcal.LabelPrintWorker
{
    using System;
    using System.Windows.Resources;
    using Shared.Workers;

    public class LabelPrintParameters : WorkerParameters
    {
        public LabelPrintParameters(IWorkerParameters parameters)
            : base(parameters)
        {
        }

        public string Address1
        {
            get { return GetParameter<string>("Address1"); }
        }

        public bool AutoPrintLabels
        {
            get { return GetParameter<bool>("AutoPrintLabels"); }
        }

        public DateTime? CalibrationTime
        {
            get { return GetParameter<DateTime?>("CalibrationTime"); }
        }

        public string CompanyName
        {
            get { return GetParameter<string>("CompanyName"); }
        }

        public string DateFormat
        {
            get { return GetParameter<string>("DateFormat"); }
        }

        public string DefaultLabelPrinter
        {
            get { return GetParameter<string>("DefaultLabelPrinter"); }
        }

        public string DocumentType
        {
            get { return GetParameter<string>("DocumentType"); }
        }

        public DateTime? ExpirationDate
        {
            get { return GetParameter<DateTime?>("ExpirationDate"); }
        }

        public int LabelNumberOfCopies
        {
            get { return GetParameter<int>("LabelNumberOfCopies"); }
        }

        public string KFactor
        {
            get { return GetParameter<string>("KFactor"); }
        }

        public string LFactor
        {
            get { return GetParameter<string>("LFactor"); }
        }

        public string LicenseKey
        {
            get { return GetParameter<string>("LicenseKey"); }
        }

        public string PhoneNumber
        {
            get { return GetParameter<string>("PhoneNumber"); }
        }

        public string PostCode
        {
            get { return GetParameter<string>("PostCode"); }
        }

        public string SealNumber
        {
            get { return GetParameter<string>("SealNumber"); }
        }

        public string SerialNumber
        {
            get { return GetParameter<string>("SerialNumber"); }
        }

        public string SkillrayTachoIcon
        {
            get { return GetParameter<string>("SkillrayTachoIcon"); }
        }

        public string TemporaryDirectory
        {
            get { return GetParameter<string>("TemporaryDirectory"); }
        }

        public string Town
        {
            get { return GetParameter<string>("Town"); }
        }

        public string TyreSize
        {
            get { return GetParameter<string>("TyreSize"); }
        }

        public string VIN
        {
            get { return GetParameter<string>("VIN"); }
        }

        public string WFactor
        {
            get { return GetParameter<string>("WFactor"); }
        }

        public string WorkshopName
        {
            get { return GetParameter<string>("WorkshopName"); }
        }
    }
}