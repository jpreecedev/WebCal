namespace Webcal.Shared.Helpers
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.IO.Compression;
    using System.Windows.Forms;

    public static class ScreenshotHelper
    {
        public static byte[] TakeScreenshot()
        {
            try
            {
                using (var capture = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(capture))
                    {
                        g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, capture.Size, CopyPixelOperation.SourceCopy);
                    }
                    using (var memoryStream = new MemoryStream())
                    {
                        capture.Save(memoryStream, ImageFormat.Jpeg);
                        return Compress(memoryStream.ToArray());
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private static byte[] Compress(byte[] rawImage)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                    {
                        gZipStream.Write(rawImage, 0, rawImage.Length);
                    }
                    return memoryStream.ToArray();
                }
            }
            catch
            {
                return rawImage;
            }
        }
    }
}