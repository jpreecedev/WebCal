namespace TachographReader.Shared.Workers.PrintWorker
{
    public class PrintParameters : WorkerParameters
    {
        public PrintParameters(IWorkerParameters parameters)
            : base(parameters)
        {
        }

        public string FilePath
        {
            get { return GetParameter<string>("FilePath"); }
        }

        public bool AlwaysAskForPrinter
        {
            get { return GetParameter<bool>("AlwaysAskForPrinter"); }
        }

        public string DefaultPrinterName
        {
            get { return GetParameter<string>("DefaultPrinterName"); }
        }

        public int DefaultNumberOfCopies
        {
            get { return GetParameter<int>("DefaultNumberOfCopies"); }
        }

        public int LabelNumberOfCopies
        {
            get { return GetParameter<int>("LabelNumberOfCopies"); }
        }

        public bool AutoClosePDFProgram
        {
            get { return GetParameter<bool>("AutoClosePDFProgram"); }
        }

        public int Timeout
        {
            get { return GetParameter<int>("Timeout"); }
        }
    }
}