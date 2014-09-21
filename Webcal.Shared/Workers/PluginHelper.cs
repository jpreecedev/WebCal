namespace Webcal.Shared.Workers
{
    using System;
    using System.Collections.Generic;

    public static class PluginHelper
    {
        private static readonly Dictionary<string, IWorker> _loadedPlugins;

        static PluginHelper()
        {
            _loadedPlugins = new Dictionary<string, IWorker>();
        }

        public static IWorker Load(string name)
        {
            var plugins = WebcalConfigurationSection.Instance.PluginsCollection;
            foreach (Plugin plugin in plugins)
            {
                if (string.Equals(plugin.Name, name))
                {
                    IWorker worker;
                    _loadedPlugins.TryGetValue(name, out worker);

                    if (worker == null)
                    {
                        worker = (IWorker) Activator.CreateInstance(Type.GetType(plugin.Type));
                        _loadedPlugins.Add(name, worker);
                    }

                    return worker;
                }
            }

            return null;
        }
    }
}