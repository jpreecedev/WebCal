namespace TachographReader.Shared.Workers
{
    using EmailWorker;
    using LabelPrintWorker;
    using PrintWorker;

    public enum WorkerTaskName
    {
        [WorkerTaskPlugin(typeof(PrintQueueWorker))]
        Print,

        [WorkerTaskPlugin(typeof(EmailQueueWorker))]
        Email,

        [WorkerTaskPlugin(typeof(LabelPrintQueueWorker))]
        LabelPrint
    }
}