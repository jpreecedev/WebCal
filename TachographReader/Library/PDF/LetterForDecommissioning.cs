namespace TachographReader.Library.PDF
{
    using System;
    using System.Drawing.Imaging;
    using System.IO;
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

    public static class LetterForDecommissioning
    {
        public static void Create(PDFDocument document, LetterForDecommissioningDocument letterForDecommissioningDocument)
        {
            var settingsRepository = ContainerBootstrapper.Resolve<ISettingsRepository<WorkshopSettings>>();
            WorkshopSettings settings = settingsRepository.GetWorkshopSettings();

            var repository = ContainerBootstrapper.Resolve<IRepository<CustomerContact>>();
            var customerContact = repository.FirstOrDefault(contact => string.Equals(letterForDecommissioningDocument.CustomerContact, contact.Name, StringComparison.CurrentCultureIgnoreCase));
            if (customerContact != null)
            {
                AbsolutePositionText(document, customerContact.Name, 61, 740, 500, 50, document.GetLargeFont(true), Element.ALIGN_LEFT);
                AbsolutePositionText(document, customerContact.Address, 61, 720, 500, 50, document.GetLargerFont(false), Element.ALIGN_LEFT);
                AbsolutePositionText(document, string.Format("{0}, {1}", customerContact.Town, customerContact.PostCode), 61, 705, 500, 50, document.GetLargerFont(false), Element.ALIGN_LEFT);
            }

            if (letterForDecommissioningDocument.InspectionDate != null)
                AbsolutePositionText(document, letterForDecommissioningDocument.InspectionDate.Value.ToString("dd.MM.yyyy"), 61, 660, 200, 20, document.GetLargerFont(false), Element.ALIGN_LEFT);

            string line1 = GetParagraph1Text(letterForDecommissioningDocument.TachographMake, letterForDecommissioningDocument.TachographModel, letterForDecommissioningDocument.SerialNumber, letterForDecommissioningDocument.RegistrationNumber);
            AbsolutePositionText(document, line1, 61, 620, 550, 50, document.GetLargerFont(false), Element.ALIGN_LEFT);

            string line2 = GetParagraph2Text();
            AbsolutePositionText(document, line2, 61, 575, 550, 50, document.GetLargerFont(false), Element.ALIGN_LEFT);

            string line3 = GetParagraph3Text();
            AbsolutePositionText(document, line3, 61, 518, 550, 100, document.GetLargerFont(false), Element.ALIGN_LEFT);

            string line4 = GetParagraph4Text();
            AbsolutePositionText(document, line4, 61, 455, 550, 100, document.GetLargerFont(false), Element.ALIGN_LEFT);

            string regardsText = GetRegardsText(letterForDecommissioningDocument.Technician);
            AbsolutePositionText(document, regardsText, 61, 395, 550, 100, document.GetLargerFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_SIGNATURE, 61, 350, 550, 100, document.GetSmallerFont(), Element.ALIGN_LEFT);
            document.DrawLine(110, 320, 350, 320);
            TryAddSignature(document, letterForDecommissioningDocument, 140, 310);

            AbsolutePositionText(document, settings.WorkshopName, 61, 200, 500, 100, document.GetLargerFont(false), Element.ALIGN_LEFT);

            string address1 = string.Format("{0}, {1}", settings.Address1, settings.Address2);
            AbsolutePositionText(document, address1, 61, 185, 500, 100, document.GetLargerFont(false), Element.ALIGN_LEFT);

            string address2 = string.Format("{0}, {1}", settings.Town, settings.PostCode);
            AbsolutePositionText(document, address2, 61, 170, 500, 100, document.GetLargerFont(false), Element.ALIGN_LEFT);
        }

        private static string GetParagraph1Text(string make, string model, string serialNumber, string registration)
        {
            return string.Format(Resources.TXT_LETTER_FOR_DECOMMISSIONING_LINE_1, make, model, serialNumber, registration);
        }

        private static string GetParagraph2Text()
        {
            return Resources.TXT_LETTER_FOR_DECOMMISSIONING_LINE_2;
        }

        private static string GetParagraph3Text()
        {
            return Resources.TXT_LETTER_FOR_DECOMMISSIONING_LINE_3;
        }

        private static string GetParagraph4Text()
        {
            return Resources.TXT_LETTER_FOR_DECOMMISSIONING_LINE_4;
        }

        private static string GetRegardsText(string technicianName)
        {
            return string.Format("{0}, {1}", Resources.TXT_REGARDS, technicianName);
        }

        private static void AbsolutePositionText(PDFDocument document, string text, float left, float top, float width, float height, Font font, int alignment)
        {
            ColumnText absoluteColumn = document.GetNewColumn(left, top, width, height);
            document.AddParagraph(text, absoluteColumn, font, alignment);
        }

        private static void TryAddSignature(PDFDocument document, LetterForDecommissioningDocument undownloadabilityDocument, int x, int y)
        {
            Image signatureImage = null;

            var userRepository = ContainerBootstrapper.Resolve<IRepository<User>>();
            User user = UserManagement.GetUser(userRepository, UserManagement.LoggedInUserName);
            if (user != null && user.Image != null)
            {
                signatureImage = user.Image;
            }

            var technicianRepository = ContainerBootstrapper.Resolve<IRepository<Technician>>();
            Technician technicianUser = technicianRepository.FirstOrDefault(c => string.Equals(c.Name, undownloadabilityDocument.Technician));
            if (technicianUser != null && technicianUser.Image != null)
            {
                signatureImage = technicianUser.Image;
            }

            if (signatureImage != null)
            {
                Image image = ImageHelper.Scale(signatureImage, 500, 50);
                document.AddImage(ToByteArray(image), image.Width, image.Height, x, y);
            }
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
    }
}