namespace TachographReader.Library.PDF
{
    using Connect.Shared;
    using Connect.Shared.Models;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using Properties;
    using Shared;
    using Shared.Helpers;
    using Image = System.Drawing.Image;

    public static class UndownloadabilityCertificate
    {
        public static void Create(PDFDocument document, UndownloadabilityDocument undownloadabilityDocument)
        {
            var settingsRepository = ContainerBootstrapper.Resolve<ISettingsRepository<WorkshopSettings>>();
            WorkshopSettings settings = settingsRepository.GetWorkshopSettings();

            var rawData = ImageHelper.LoadFromResourcesAsByteArray("UndownloadHeader");
            document.AddImage(rawData, 390, 290, 59, document.Height - 350);

            AbsolutePositionText(document, settings.WorkshopName, 61, 710, 500, 50, document.GetLargeFont(true), Element.ALIGN_LEFT);
            AbsolutePositionText(document, settings.Address1, 61, 680, 500, 50, document.GetRegularFont(false), Element.ALIGN_LEFT);
            AbsolutePositionText(document, $"{settings.Town} {settings.PostCode}", 61, 645, 500, 50, document.GetRegularFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, undownloadabilityDocument.TachographMake, 61, 585, 500, 50, document.GetRegularFont(false), Element.ALIGN_LEFT);
            AbsolutePositionText(document, undownloadabilityDocument.TachographModel, 61, 548, 500, 50, document.GetRegularFont(false), Element.ALIGN_LEFT);
            AbsolutePositionText(document, undownloadabilityDocument.SerialNumber, 61, 510, 500, 50, document.GetRegularFont(false), Element.ALIGN_LEFT);

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

            TryAddSignature(document, undownloadabilityDocument, 310, 158);
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
            document.AddParagraph(text, absoluteColumn, font, BaseColor.BLACK, alignment);
        }

        private static void TryAddSignature(PDFDocument document, UndownloadabilityDocument undownloadabilityDocument, int x, int y)
        {
            Image signatureImage = null;

            var userRepository = ContainerBootstrapper.Resolve<IRepository<User>>();
            var user = UserManagement.GetUser(userRepository, UserManagement.LoggedInUserName);
            if (user != null && user.Image != null)
            {
                signatureImage = user.Image;
            }

            IRepository<Technician> technicianRepository = ContainerBootstrapper.Resolve<IRepository<Technician>>();
            var technicianUser = technicianRepository.FirstOrDefault(c => string.Equals(c.Name, undownloadabilityDocument.Technician));
            if (technicianUser != null && technicianUser.Image != null)
            {
                signatureImage = technicianUser.Image;
            }

            if (signatureImage != null)
            {
                Image image = ImageHelper.Scale(signatureImage,500, 50);
                document.AddImage(ImageHelper.ToByteArray(image), image.Width, image.Height, x, y);
            }
        }
    }
}