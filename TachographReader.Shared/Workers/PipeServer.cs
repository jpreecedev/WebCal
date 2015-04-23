namespace TachographReader.Shared.Workers
{
    using System;

    public class PipeServer : BasePipeProvider, IPipeServer
    {
        private readonly Type _pluginType;

        public PipeServer(Type pluginType)
        {
            _pluginType = pluginType;
        }

        public void Connect(IWorkerParameters parameters)
        {
            try
            {
                var plugin = (IWorker) Activator.CreateInstance(_pluginType);
                plugin.Start(parameters);
            }
            catch (Exception ex)
            {
                OnChanged(Error, ex.Message);
                return;
            }

            OnChanged(Completed, string.Empty);
        }
    }
}