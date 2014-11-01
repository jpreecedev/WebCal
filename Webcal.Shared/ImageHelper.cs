namespace Webcal.Shared
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using Properties;

    public static class ImageHelper
    {
        private static readonly string TempByEnvironmentVariable = Environment.GetEnvironmentVariable("TEMP");
        private static readonly string TempByPath = Path.GetTempPath();

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

            double aspectRatio = Math.Round((double)image.Width / image.Height, 2);
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

            Image image = LoadImageSafely(path);
            if (image == null)
            {
                throw new InvalidOperationException(Resources.EXC_IMAGE_COULD_NOT_BE_LOADED);
            }

            if (image.Height <= maxHeight) //No scaling required
            {
                return image;
            }

            double aspectRatio = Math.Round((double)image.Width / image.Height, 2);
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