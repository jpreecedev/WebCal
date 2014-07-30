using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace Webcal.Shared
{
    public static class ScreenshotHelper
    {
        public static byte[] TakeScreenshot()
        {
            try
            {
                using (Bitmap capture = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(capture))
                    {
                        g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, capture.Size, CopyPixelOperation.SourceCopy);
                    }
                    using (MemoryStream memoryStream = new MemoryStream())
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
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
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