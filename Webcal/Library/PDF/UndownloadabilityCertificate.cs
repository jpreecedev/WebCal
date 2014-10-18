namespace Webcal.Library.PDF
{
    using System;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Resources;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using Properties;
    using Shared;
    using Image = System.Drawing.Image;

    public static class UndownloadabilityCertificate
    {
        public static void Create(PDFDocument document, UndownloadabilityDocument undownloadabilityDocument)
        {
            var settingsRepository = ContainerBootstrapper.Container.GetInstance<ISettingsRepository<WorkshopSettings>>();
            WorkshopSettings settings = settingsRepository.GetWorkshopSettings();

            StreamResourceInfo resourceStream = DocumentHelper.GetResourceStreamFromSimplePath("Images/PDF/UndownloadHeader.png");

            var rawData = new byte[resourceStream.Stream.Length];
            resourceStream.Stream.Read(rawData, 0, rawData.Length);
            document.AddImage(rawData, 390, 290, 59, document.Height - 350);

            AbsolutePositionText(document, settings.WorkshopName, 61, 710, 500, 50, document.GetLargeFont(true), Element.ALIGN_LEFT);
            AbsolutePositionText(document, settings.Address1, 61, 680, 500, 50, document.GetRegularFont(false), Element.ALIGN_LEFT);
            AbsolutePositionText(document, string.Format("{0} {1}", settings.Town, settings.PostCode), 61, 645, 500, 50, document.GetRegularFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_TACHOGRAPH_MAKE, 61, 585, 500, 50, document.GetRegularFont(false), Element.ALIGN_LEFT);
            AbsolutePositionText(document, Resources.TXT_TACHOGRAPH_MODEL, 61, 548, 500, 50, document.GetRegularFont(false), Element.ALIGN_LEFT);
            AbsolutePositionText(document, Resources.TXT_TACHOGRAPH_NUMBER, 61, 510, 500, 50, document.GetRegularFont(false), Element.ALIGN_LEFT);

            string line1 = GetLine1Text(undownloadabilityDocument.TachographMake, undownloadabilityDocument.TachographModel, undownloadabilityDocument.SerialNumber, undownloadabilityDocument.RegistrationNumber);
            AbsolutePositionText(document, line1, 61, 450, 550, 50, document.GetLargerFont(false), Element.ALIGN_LEFT);

            string line2 = GetLine2Text();
            AbsolutePositionText(document, line2, 61, 405, 550, 50, document.GetLargerFont(false), Element.ALIGN_LEFT);

            string line3 = GetLine3Text();
            AbsolutePositionText(document, line3, 61, 375, 550, 100, document.GetLargerFont(false), Element.ALIGN_LEFT);

            string line4 = GetLine4Text();
            AbsolutePositionText(document, line4, 61, 310, 550, 100, document.GetLargerFont(false), Element.ALIGN_LEFT);

            document.DrawLine(50, 219, 545, 219);
            document.DrawLine(50, 147, 545, 147);
            document.DrawLine(125, 172, 238, 172);
            document.DrawLine(322, 172, 484, 172);

            if (undownloadabilityDocument.InspectionDate != null)
            {
                AbsolutePositionText(document, undownloadabilityDocument.InspectionDate.Value.ToString("dd.MM.yyyy"), 125, 195, 200, 20, document.GetSmallerFont(), Element.ALIGN_LEFT);
            }

            AbsolutePositionText(document, Resources.TXT_DATE, 125, 175, 200, 20, document.GetSmallerFont(), Element.ALIGN_LEFT);

            TryAddSignature(document, 310, 170);
            AbsolutePositionText(document, string.Format(Resources.TXT_UNDOWNLOADABILITY_SIGNATURE, undownloadabilityDocument.Technician), 322, 175, 522, 20, document.GetSmallerFont(), Element.ALIGN_LEFT);
        }

        private static string GetLine1Text(string make, string model, string serialNumber, string registration)
        {
            return string.Format(Resources.TXT_UNDOWNLOADABILITY_LINE_1, make, model, serialNumber, registration);
        }

        private static string GetLine2Text()
        {
            return Resources.TXT_UNDOWNLOADABILITY_LINE_2;
        }

        private static string GetLine3Text()
        {
            return Resources.TXT_UNDOWNLOADABILITY_LINE_3;
        }

        private static string GetLine4Text()
        {
            return Resources.TXT_UNDOWNLOADABILITY_LINE_4;
        }

        private static void AbsolutePositionText(PDFDocument document, string text, float left, float top, float width, float height, Font font, int alignment)
        {
            ColumnText absoluteColumn = document.GetNewColumn(left, top, width, height);
            document.AddParagraph(text, absoluteColumn, font, alignment);
        }

        private static byte[] ToByteArray(Image imageIn)
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

        private static void TryAddSignature(PDFDocument document, int x, int y)
        {
            var userRepository = ContainerBootstrapper.Container.GetInstance<IRepository<User>>();
            User user = UserManagement.GetUser(userRepository, UserManagement.LoggedInUserName);
            if (user != null && user.Image != null)
            {
                Image image = ImageHelper.Scale(user.Image, 50);
                document.AddImage(ToByteArray(image), image.Width, image.Height, x, y);
            }
        }
    }
}