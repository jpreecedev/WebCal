namespace Webcal.Library
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows;
    using System.Windows.Resources;
    using Properties;

    public static class DocumentHelper
    {
        private static readonly string TempByEnvironmentVariable = Environment.GetEnvironmentVariable("TEMP");
        private static readonly string TempByPath = Path.GetTempPath();

        public static string GetPlainTextDocumentFromResource(string simplePath)
        {
            if (string.IsNullOrEmpty(simplePath))
            {
                throw new ArgumentNullException("simplePath");
            }

            StreamResourceInfo streamResourceInfo = GetResourceStreamFromSimplePath(simplePath);
            using (var reader = new StreamReader(streamResourceInfo.Stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static void SaveImageToDisk(Image image, string destinationPath)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            if (string.IsNullOrEmpty(destinationPath))
            {
                throw new ArgumentNullException("destinationPath");
            }

            image.Save(destinationPath, image.RawFormat);
        }

        public static string GetTemporaryDirectory()
        {
            if (!string.IsNullOrEmpty(TempByEnvironmentVariable))
            {
                return TempByEnvironmentVariable;
            }

            if (!string.IsNullOrEmpty(TempByPath))
            {
                return TempByPath;
            }

            return string.Empty;
        }

        public static StreamResourceInfo GetResourceStreamFromSimplePath(string simplePath)
        {
            if (simplePath.StartsWith("../"))
            {
                simplePath = simplePath.Substring(3, simplePath.Length - 3);
            }

            StreamResourceInfo resourceInfo = Application.GetResourceStream(new Uri(string.Format("pack://application:,,,/{0}", simplePath), UriKind.Absolute));

            if (resourceInfo == null)
            {
                throw new InvalidOperationException(string.Format(Resources.EXC_COULD_NOT_LOCATE_RESOURCE, simplePath));
            }

            return resourceInfo;
        }

        public static string GetResourceFromSimplePath(string simplePath)
        {
            if (simplePath.StartsWith("../"))
            {
                simplePath = simplePath.Substring(3, simplePath.Length - 3);
            }

            StreamResourceInfo resourceInfo = Application.GetResourceStream(new Uri(string.Format("pack://application:,,,/{0}", simplePath), UriKind.Absolute));

            if (resourceInfo == null)
            {
                throw new InvalidOperationException(string.Format(Resources.EXC_COULD_NOT_LOCATE_RESOURCE, simplePath));
            }

            var image = Image.FromStream(resourceInfo.Stream);
            var path = Path.Combine(GetTemporaryDirectory(), Guid.NewGuid() + Path.GetExtension(simplePath));
            image.Save(path);

            return path;
        }
    }
}