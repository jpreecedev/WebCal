namespace TachographReader.Library
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Windows;
    using System.Windows.Resources;
    using Properties;
    using SevenZip;
    using Shared;
    using Shared.Helpers;

    public static class DocumentHelper
    {
        public static string GetPlainTextDocumentFromResource(string simplePath)
        {
            if (string.IsNullOrEmpty(simplePath))
            {
                throw new ArgumentNullException(nameof(simplePath));
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

            StreamResourceInfo resourceInfo = Application.GetResourceStream(new Uri($"pack://application:,,,/{simplePath}", UriKind.Absolute));

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

            StreamResourceInfo resourceInfo = Application.GetResourceStream(new Uri($"pack://application:,,,/{simplePath}", UriKind.Absolute));

            if (resourceInfo == null)
            {
                throw new InvalidOperationException(string.Format(Resources.EXC_COULD_NOT_LOCATE_RESOURCE, simplePath));
            }

            var image = Image.FromStream(resourceInfo.Stream);
            var path = Path.Combine(ImageHelper.GetTemporaryDirectory(), Guid.NewGuid() + Path.GetExtension(simplePath));
            image.Save(path);

            return path;
        }

        public static byte[] Zip(string filePath)
        {
            var tempFileName = DateTime.Now.ToString("ddMMyyyyHHmmss");
            var tempDirectory = Path.Combine(ImageHelper.GetTemporaryDirectory(), tempFileName);
            var originalFileName = Path.GetFileName(filePath);
            var finalDestination = Path.Combine(ImageHelper.GetTemporaryDirectory(), tempFileName + ".zip");

            Directory.CreateDirectory(tempDirectory);
            File.Copy(filePath, Path.Combine(tempDirectory, originalFileName));

            SevenZipBase.SetLibraryPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.dll"));
            var compressor = new SevenZipCompressor
            {
                ArchiveFormat = OutArchiveFormat.Zip,
                CompressionMode = CompressionMode.Create,
                CompressionLevel = CompressionLevel.Ultra,
                TempFolderPath = tempDirectory
            };
            compressor.CompressDirectory(tempDirectory, finalDestination);
            
            return File.ReadAllBytes(finalDestination);
        }
    }
}