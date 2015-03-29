namespace Webcal.Library
{
    using System.Reflection;
    using System.Resources;

    public static class LocalizationHelper
    {
        private static readonly ResourceManager _resourceManager;

        static LocalizationHelper()
        {
            _resourceManager = new ResourceManager("Webcal.Properties.Resources", Assembly.GetEntryAssembly());
        }

        public static ResourceManager GetResourceManager()
        {
            return _resourceManager;
        }
    }
}