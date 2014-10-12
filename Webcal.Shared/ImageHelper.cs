namespace Webcal.Shared
{
    using System;
    using System.Drawing;
    using Properties;

    public static class ImageHelper
    {
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

            double aspectRatio = Math.Round((double) image.Width/image.Height, 2);
            var newWidth = (int) Math.Round(aspectRatio*50);

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

            double aspectRatio = Math.Round((double) image.Width/image.Height, 2);
            var newWidth = (int) Math.Round(aspectRatio*50);

            return Resize(image, new Size(newWidth, maxHeight));
        }

        private static Bitmap Resize(Image image, Size size)
        {
            return new Bitmap(image, size);
        }
    }
}