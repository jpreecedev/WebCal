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

        public static string CopyResourceToFileSystem(string key, Assembly assembly = null)
        {
            var resourceManager = new ResourceManager("TachographReader.Properties.Resources", assembly ?? Assembly.GetCallingAssembly());
            var image = resourceManager.GetObject(key, CultureInfo.CurrentUICulture);

            if (image == null)
            {
                return null;
            }

            var bitmap = (Bitmap)image;
            var imagePath = Path.Combine(GetTemporaryDirectory(), Guid.NewGuid() + ".png");
            SaveImageToDisk(bitmap, imagePath);

            return imagePath;
        }

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

        public static Image Scale(Image image, int maxWidth, int maxHeight)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }
            if (maxHeight < 0)
            {
                throw new ArgumentNullException(nameof(maxHeight));
            }

            if (image.Height <= maxHeight) //No scaling required
            {
                return image;
            }

            return Resize(image, ResizeKeepAspect(image.Size, maxWidth, maxHeight));
        }

        public static Image Scale(string path, int maxWidth, int maxHeight)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (maxHeight < 0)
            {
                throw new ArgumentNullException(nameof(maxHeight));
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
            
            return Resize(image, ResizeKeepAspect(image.Size, maxWidth, maxHeight));
        }

        public static void SaveImageToDisk(Image image, string destinationPath)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }
            if (string.IsNullOrEmpty(destinationPath))
            {
                throw new ArgumentNullException(nameof(destinationPath));
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
                return (Bitmap)(new ImageConverter().ConvertFrom(rawData));
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
                    imageIn.Save(ms, ImageFormat.Jpeg);
                    return ms.ToArray();
                }
            }
            catch (Exception)
            {
                //Generic GDI+ error ... no point in logging as the error is meaningless
            }

            return null;
        }

        private static Size ResizeKeepAspect(Size currentDimensions, int maxWidth, int maxHeight)
        {
            int newHeight = currentDimensions.Height;
            int newWidth = currentDimensions.Width;
            if (maxWidth > 0 && newWidth > maxWidth) //WidthResize
            {
                Decimal divider = Math.Abs((decimal)newWidth / maxWidth);
                newWidth = maxWidth;
                newHeight = (int)Math.Round(newHeight / divider);
            }
            if (maxHeight > 0 && newHeight > maxHeight) //HeightResize
            {
                Decimal divider = Math.Abs((decimal)newHeight / maxHeight);
                newHeight = maxHeight;
                newWidth = (int)Math.Round(newWidth / divider);
            }
            return new Size(newWidth, newHeight);
        }
    }
}