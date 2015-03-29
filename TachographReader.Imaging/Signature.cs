using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace TachographReader.Imaging
{
    public static class Signature
    {
        private static readonly string TempByEnvironmentVariable = Environment.GetEnvironmentVariable("TEMP");
        private static readonly string TempByPath = Path.GetTempPath();
        
        public static string Transform(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            bitmap = ApplyFilter(Grayscale.CommonAlgorithms.BT709, bitmap);
            bitmap = ApplyFilter(new GrayscaleToRGB(), bitmap);

            ContrastCorrection filter = new ContrastCorrection
                                            {
                                                Factor = Math.Max(Math.Min(30, 127), -127)
                                            };

            bitmap = ApplyFilter(filter, bitmap);
            return SaveImage(bitmap);
        }

        private static Bitmap ApplyFilter(IFilter filter, Bitmap image)
        {
            IFilterInformation filterInformation = filter as IFilterInformation;
            if (filterInformation != null && !filterInformation.FormatTranslations.ContainsKey(image.PixelFormat))
            {
                if (filterInformation.FormatTranslations.ContainsKey(PixelFormat.Format24bppRgb))
                    return null;

                return null;
            }
            return filter.Apply(image);
        }

        private static string SaveImage(Image image)
        {
            string path = "thesig" + DateTime.Now.Ticks + ".jpg";
            image.Save(Path.Combine(GetTemporaryDirectory(), path), ImageFormat.Jpeg);
            return Path.Combine(GetTemporaryDirectory(), path);
        }

        private static string GetTemporaryDirectory()
        {
            if (!string.IsNullOrEmpty(TempByEnvironmentVariable))
                return TempByEnvironmentVariable;

            if (!string.IsNullOrEmpty(TempByPath))
                return TempByPath;

            return string.Empty;
        }
    }
}
