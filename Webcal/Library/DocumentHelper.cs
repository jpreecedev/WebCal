namespace Webcal.Library
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Windows;
    using System.Windows.Resources;
    using Properties;
    using Shared;

    public static class DocumentHelper
    {
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
            var path = Path.Combine(ImageHelper.GetTemporaryDirectory(), Guid.NewGuid() + Path.GetExtension(simplePath));
            image.Save(path);

            return path;
        }
    }
}