namespace Webcal.Library.PDF
{
    using System;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Windows.Resources;
    using Core;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using Properties;
    using Shared;
    using StructureMap;
    using Image = System.Drawing.Image;

    public class BasePlaqueDocument : IPlaque
    {
        public const int TOTAL_PAGE_HEIGHT = 820;
        private readonly IGeneralSettingsRepository _generalSettingsRepository;

        public BasePlaqueDocument()
        {
            _generalSettingsRepository = ObjectFactory.GetInstance<IGeneralSettingsRepository>();
            WorkshopSettings = _generalSettingsRepository.GetSettings();
            RegistrationData = GetRegistrationData();
        }

        protected RegistrationData RegistrationData { get; set; }
        protected CustomerContact CustomerContact { get; set; }
        protected WorkshopSettings WorkshopSettings { get; set; }

        public void Create(PDFDocument pdfDocument, TachographDocument tachographDocument)
        {
            ////SCALE!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //int x = 20;
            //int y = 0;
            //while (y < 800)
            //{
            //    AbsolutePositionText(pdfDocument, y.ToString(), (x), (y), 0, 0, pdfDocument.GetSmallFont(false));
            //    y += 10;
            //}
            //while (x < 600)
            //{
            //    AbsolutePositionText(pdfDocument, x.ToString(), (x), (0), 595, 0, pdfDocument.GetSmallFont(false));
            //    x += 30;
            //}    

            //AddTitle(pdfDocument, GetTitle());
            CustomerContact = _generalSettingsRepository.GetCustomerSettings(tachographDocument.CustomerContact);
            //CreateSmallLabel(pdfDocument, tachographDocument);
            //CreateMediumLabel(pdfDocument, tachographDocument);
            //CreateKFactorLabel(pdfDocument, tachographDocument);
            CreateLargeLabelLogos(pdfDocument, tachographDocument, 0, 10);
            CreateLargeLabelAddress(pdfDocument, tachographDocument, 0, 230);
            CreateLargeLabel(pdfDocument, tachographDocument, 0, 400);
            CreateLargeLabelExpiry(pdfDocument, tachographDocument, 0, 360);
        }

        public void CreateFullCertificate(PDFDocument pdfDocument, TachographDocument tachographDocument)
        {
            CustomerContact = _generalSettingsRepository.GetCustomerSettings(tachographDocument.CustomerContact);
            CreateLargeCertificate(pdfDocument, tachographDocument);
        }


        protected virtual string GetTitle()
        {
            return string.Empty;
        }

        protected void GetCompanyDetails(PDFDocument document, PdfPTable table, int height, int colspan, Font font)
        {
            string companyName = Resources.TXT_SKILLRAY;
            string address = Resources.TXT_UNLICENSED;
            string town = Resources.TXT_DEMO;
            string postCode = Resources.TXT_SOFTWARE;

            if (!string.IsNullOrEmpty(RegistrationData.LicenseKey) || RegistrationData.ExpirationDate < DateTime.Now.Date)
            {
                companyName = WorkshopSettings.WorkshopName;
                address = WorkshopSettings.Address1;
                town = WorkshopSettings.Town;
                postCode = WorkshopSettings.PostCode;
            }

            document.AddSpannedCell(table, string.Format("{0}\n{1} {2} {3}", companyName, address, town, postCode), colspan, font, height, Element.ALIGN_LEFT); //69
        }

        protected void GetWorkshopImage(PDFDocument document, PdfPTable table)
        {
            DocumentHelper.SaveImageToDisk(GetWorkshopImage(), Path.Combine(DocumentHelper.GetTemporaryDirectory(), "logo2.png"));
            iTextSharp.text.Image itextSharpImage = document.GetImage(Path.Combine(DocumentHelper.GetTemporaryDirectory(), "logo2.png"), 195, 51);
            document.AddSpannedCell(table, itextSharpImage, 2, document.GetRegularFont(false), 29);
        }

        protected void GetSmallImage(PDFDocument document, PdfPTable table)
        {
            DocumentHelper.SaveImageToDisk(GetWorkshopImage(), Path.Combine(DocumentHelper.GetTemporaryDirectory(), "logo.png"));
            iTextSharp.text.Image itextSharpImage = document.GetImage(Path.Combine(DocumentHelper.GetTemporaryDirectory(), "logo.png"), 205, 53);
            document.AddSpannedCell(table, itextSharpImage, 4, document.GetRegularFont(false), 40);
        }

        protected Image GetWorkshopImage()
        {
            StreamResourceInfo resourceStream = DocumentHelper.GetResourceStreamFromSimplePath("Images/PDF/skillray-small1.png");
            return Image.FromStream(resourceStream.Stream);
        }

        protected string TrimDocumentType(string type)
        {
            return type.Substring(3, type.Length - 3);
        }

        protected string GetCalibrationTime(DateTime? calibrationTime)
        {
            if (calibrationTime == null)
                return DateTime.Now.ToString(Constants.DateFormat);

            return calibrationTime.Value.ToString(Constants.DateFormat);
        }

        protected Paragraph GetLicenseNumberParagraph(PDFDocument document, bool small)
        {
            Font smallFont = small ? document.GetSmallFont(false) : document.GetRegularFont(false);
            Font regularFont = small ? document.GetSmallFont(true) : document.GetRegularFont(true);

            var paragraph = new Paragraph();
            paragraph.Add(new Chunk(Resources.TXT_LICENSE_NO_MINIMAL, smallFont));
            paragraph.Add(new Chunk(RegistrationData.SealNumber, regularFont)); // License
            return paragraph;
        }

        protected void AddTitle(PDFDocument document, string title)
        {
            var paragraph = new Paragraph(title, document.GetXLargeFont(false));
            paragraph.Alignment = Element.ALIGN_CENTER;
            document.Document.Add(paragraph);
        }

        protected void AbsolutePositionText(PDFDocument document, string text, float left, float top, float width, float height)
        {
            ColumnText titleHeaderText = document.GetNewColumn(left, (TOTAL_PAGE_HEIGHT - top), width, height);
            document.AddParagraph(text, titleHeaderText, document.GetRegularFont(false));
        }

        protected void AbsolutePositionText(PDFDocument document, string text, float left, float top, float width, float height, Font font)
        {
            ColumnText absoluteColumn = document.GetNewColumn(left, (TOTAL_PAGE_HEIGHT - top), width, height);
            document.AddParagraph(text, absoluteColumn, font);
        }

        protected void AbsolutePositionText(PDFDocument document, string text, float left, float top, float width, float height, Font font, int alignment)
        {
            ColumnText absoluteColumn = document.GetNewColumn(left, (TOTAL_PAGE_HEIGHT - top), width, height);
            document.AddParagraph(text, absoluteColumn, font, alignment);
        }

        protected virtual void CreateLargeCertificate(PDFDocument document, TachographDocument tachographDocument)
        {
        }

        protected virtual void CreateLargeLabel(PDFDocument document, TachographDocument tachographDocument, int startHorizontal, int startVertical)
        {
        }

        protected virtual void CreateLargeLabelAddress(PDFDocument document, TachographDocument tachographDocument, int startHorizontal, int startVertical)
        {
        }

        protected virtual void CreateLargeLabelExpiry(PDFDocument document, TachographDocument tachographDocument, int startHorizontal, int startVertical)
        {
        }

        protected virtual void CreateLargeLabelLogos(PDFDocument document, TachographDocument tachographDocument, int startHorizontal, int startVertical)
        {
        }

        protected virtual void DrawLargeLabelRectangle(PDFDocument document, int startHorizontal, int startVertical)
        {
        }

        protected virtual void CreateKFactorLabel(PDFDocument document, TachographDocument tachographDocument)
        {
        }

        protected virtual void CreateMediumLabel(PDFDocument document, TachographDocument tachographDocument)
        {
        }

        protected virtual void CreateSmallLabel(PDFDocument document, TachographDocument tachographDocument)
        {
        }

        private static RegistrationData GetRegistrationData()
        {
            return ObjectFactory.GetInstance<IRepository<RegistrationData>>().GetAll().First();
        }

        protected static byte[] ToByteArray(Image imageIn)
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

        protected void TryAddSignature(PDFDocument document, int x, int y)
        {
            var userRepository = ObjectFactory.GetInstance<IRepository<User>>();
            User user = UserManagement.GetUser(userRepository, UserManagement.LoggedInUserName);
            if (user != null && user.Image != null)
            {
                Image image = ImageHelper.Scale(user.Image, 50);
                document.AddImage(ToByteArray(image), image.Width, image.Height, x, 80);
            }
        }
    }
}