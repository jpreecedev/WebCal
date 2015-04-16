namespace TachographReader.Shared.Helpers
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Resources;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media.Imaging;
    using Properties;
    using Size = System.Drawing.Size;

    public static class ImageHelper
    {
        private static readonly string TempByEnvironmentVariable = Environment.GetEnvironmentVariable("TEMP");
        private static readonly string TempByPath = Path.GetTempPath();

        public static BitmapSource LoadFromResources(string key, Assembly assembly = null)
        {
            var resourceManager = new ResourceManager("TachographReader.Properties.Resources", assembly ?? Assembly.GetCallingAssembly());
            var image = resourceManager.GetObject(key, CultureInfo.CurrentUICulture);

            if (image == null)
            {
                return null;
            }

            var bitmap = (Bitmap)image;

            return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
        
        public static byte[] LoadFromResourcesAsByteArray(string key, Assembly assembly = null)
        {
            var resourceManager = new ResourceManager("TachographReader.Properties.Resources", assembly ?? Assembly.GetCallingAssembly());
            var image = resourceManager.GetObject(key, CultureInfo.CurrentUICulture);

            if (image == null)
            {
                return null;
            }

            var bitmap = (Bitmap)image;
            var asByteArray = bitmap.ToByteArray();
            return asByteArray;
        }

        public static Image LoadImageSafely(string imagePath)
        {
            using (var bmpTemp = new Bitmap(imagePath))
            {
                return new Bitmap(bmpTemp);
            }
        }

        public static Image Scale(Image image, int maxHeight)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }
            if (maxHeight < 0)
            {
                throw new ArgumentNullException("maxHeight");
            }

            if (image.Height <= maxHeight) //No scaling required
            {
                return image;
            }

            var aspectRatio = Math.Round((double)image.Width / image.Height, 2);
            var newWidth = (int)Math.Round(aspectRatio * 50);

            return Resize(image, new Size(newWidth, maxHeight));
        }

        public static Image Scale(string path, int maxHeight)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }
            if (maxHeight < 0)
            {
                throw new ArgumentNullException("maxHeight");
            }

            var image = LoadImageSafely(path);
            if (image == null)
            {
                throw new InvalidOperationException(Resources.EXC_IMAGE_COULD_NOT_BE_LOADED);
            }

            if (image.Height <= maxHeight) //No scaling required
            {
                return image;
            }

            var aspectRatio = Math.Round((double)image.Width / image.Height, 2);
            var newWidth = (int)Math.Round(aspectRatio * 50);

            return Resize(image, new Size(newWidth, maxHeight));
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

        private static Image Resize(Image image, Size size)
        {
            return new Bitmap(image, size);
        }

        public static Image ToImage(this byte[] rawData)
        {
            try
            {
                return (Bitmap)((new ImageConverter()).ConvertFrom(rawData));
            }
            catch (Exception)
            {
                //Generic GDI+ error ... no point in logging as the error is meaningless
            }

            return null;
        }

        public static byte[] ToByteArray(this BitmapSource bitmapSource)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            byte[] result;
            using (MemoryStream stream = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(stream);

                result = stream.ToArray();
                stream.Flush();
                stream.Close();
            }
            return result;
        }

        public static byte[] ToByteArray(this Image imageIn)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    imageIn.Save(ms, ImageFormat.Bmp);
                    return ms.ToArray();
                }
            }
            catch (Exception)
            {
                //Generic GDI+ error ... no point in logging as the error is meaningless
            }

            return null;
        }
    }
}