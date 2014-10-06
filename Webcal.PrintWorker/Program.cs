namespace Webcal.PrintWorker
{
    using Shared.Workers;

    public class Program : BaseWorkerEntryPoint
    {
        public static void Main(string[] args)
        {
            using (IPipeClient client = new PipeClient(args[0]))
            {
                client.Connect();

                var worker = new PrintQueueWorker(client.SendMessage);
                worker.ProgressChanged += (sender, eventArgs) => client.ProgressChanged(sender, eventArgs);
                worker.Completed += (sender, eventArgs) => client.Completed(sender, eventArgs);

                worker.Start(ProcessParameters(args));
            }
        }
    }
}