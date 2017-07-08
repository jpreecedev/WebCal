namespace TachographReader.Library.PDF
{
    using System;
    using System.IO;
    using System.Linq;
    using Connect.Shared;
    using Connect.Shared.Models;
    using Core;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using DataModel.Models;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using Properties;
    using Shared;
    using Shared.Helpers;
    using Image = System.Drawing.Image;

    public class BasePlaqueDocument : IPlaque
    {
        protected const int TotalPageHeight = 820;
        private readonly IRepository<CustomerContact> _customerContactRepository;

        public BasePlaqueDocument()
        {
            ISettingsRepository<WorkshopSettings> generalSettingsRepository = ContainerBootstrapper.Resolve<ISettingsRepository<WorkshopSettings>>();
            ISettingsRepository<AdvertisingSettings> advertisingSettingsRepository = ContainerBootstrapper.Resolve<ISettingsRepository<AdvertisingSettings>>();
            _customerContactRepository = ContainerBootstrapper.Resolve<IRepository<CustomerContact>>();

            WorkshopSettings = generalSettingsRepository.GetWorkshopSettings();
            AdvertisingSettings = advertisingSettingsRepository.GetAdvertisingSettings();
            RegistrationData = GetRegistrationData();
        }

        protected RegistrationData RegistrationData { get; set; }
        protected CustomerContact CustomerContact { get; set; }
        protected WorkshopSettings WorkshopSettings { get; set; }
        protected AdvertisingSettings AdvertisingSettings { get; set; }

        public void Create(PDFDocument pdfDocument, TachographDocument tachographDocument)
        {
            CustomerContact = _customerContactRepository.FirstOrDefault(c => c.Name == tachographDocument.CustomerContact);
            CreateLargeLabelLogos(pdfDocument, tachographDocument, 0, 10);
            CreateLargeLabelAddress(pdfDocument, tachographDocument, 0, 230);
            CreateLargeLabel(pdfDocument, tachographDocument, 0, 400);
            CreateLargeLabelExpiry(pdfDocument, tachographDocument, 0, 360);
        }

        public void CreateFullCertificate(PDFDocument pdfDocument, TachographDocument tachographDocument, bool excludeLogos)
        {
            CustomerContact = _customerContactRepository.FirstOrDefault(c => c.Name == tachographDocument.CustomerContact);
            CreateLargeCertificate(pdfDocument, tachographDocument, excludeLogos);
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

            if (RegistrationData.HasValidLicense)
            {
                companyName = WorkshopSettings.WorkshopName;
                address = WorkshopSettings.Address1;
                town = WorkshopSettings.Town;
                postCode = WorkshopSettings.PostCode;
            }

            document.AddSpannedCell(table, $"{companyName}\n{address} {town} {postCode}", colspan, font, height, Element.ALIGN_LEFT); //69
        }

        protected void GetWorkshopImage(PDFDocument document, PdfPTable table)
        {
            ImageHelper.SaveImageToDisk(GetWorkshopImage(), Path.Combine(ImageHelper.GetTemporaryDirectory(), "logo2.png"));
            iTextSharp.text.Image itextSharpImage = document.GetImage(Path.Combine(ImageHelper.GetTemporaryDirectory(), "logo2.png"), 195, 51);
            document.AddSpannedCell(table, itextSharpImage, 2, document.GetRegularFont(false), 29);
        }

        protected void GetSmallImage(PDFDocument document, PdfPTable table)
        {
            ImageHelper.SaveImageToDisk(GetWorkshopImage(), Path.Combine(ImageHelper.GetTemporaryDirectory(), "logo.png"));
            iTextSharp.text.Image itextSharpImage = document.GetImage(Path.Combine(ImageHelper.GetTemporaryDirectory(), "logo.png"), 205, 53);
            document.AddSpannedCell(table, itextSharpImage, 4, document.GetRegularFont(false), 40);
        }

        protected Image GetWorkshopImage()
        {
            return ImageHelper.LoadFromResources("skillray_small1").ToBitmap();
        }

        protected string TrimDocumentType(string type)
        {
            return type.Substring(3, type.Length - 3);
        }

        protected string GetCalibrationTime(DateTime? calibrationTime)
        {
            if (calibrationTime == null)
            {
                return DateTime.Now.ToString(Constants.DateFormat);
            }

            return calibrationTime.Value.ToString(Constants.DateFormat);
        }

        protected Paragraph GetLicenseNumberParagraph(PDFDocument document, bool small)
        {
            Font smallFont = small ? document.GetSmallFont(false) : document.GetRegularFont(false);
            Font regularFont = small ? document.GetSmallFont(true) : document.GetRegularFont(true);

            return new Paragraph
            {
                new Chunk(Resources.TXT_LICENSE_NO_MINIMAL, smallFont),
                new Chunk(RegistrationData.SealNumber, regularFont)
            };
        }

        protected void AddTitle(PDFDocument document, string title)
        {
            var paragraph = new Paragraph(title, document.GetXLargeFont(false))
            {
                Alignment = Element.ALIGN_CENTER
            };

            document.Document.Add(paragraph);
        }

        protected void AbsolutePositionText(PDFDocument document, string text, float left, float top, float width, float height)
        {
            ColumnText titleHeaderText = document.GetNewColumn(left, (TotalPageHeight - top), width, height);
            document.AddParagraph(text, titleHeaderText, document.GetRegularFont(false));
        }

        protected void AbsolutePositionText(PDFDocument document, string text, float left, float top, float width, float height, Font font)
        {
            ColumnText absoluteColumn = document.GetNewColumn(left, (TotalPageHeight - top), width, height);
            document.AddParagraph(text, absoluteColumn, font);
        }

        protected void AbsolutePositionText(PDFDocument document, string text, float left, float top, float width, float height, Font font, int alignment)
        {
            ColumnText absoluteColumn = document.GetNewColumn(left, (TotalPageHeight - top), width, height);
            document.AddParagraph(text, absoluteColumn, font, BaseColor.BLACK, alignment);
        }

        protected virtual void CreateLargeCertificate(PDFDocument document, TachographDocument tachographDocument, bool excludeLogos)
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
            return ContainerBootstrapper.Resolve<IRepository<RegistrationData>>().GetAll().First();
        }

        protected void TryAddSignature(PDFDocument document, TachographDocument tachographDocument, int x, int y = 88)
        {
            Image signatureImage = null;

            var userRepository = ContainerBootstrapper.Resolve<IRepository<User>>();
            var user = UserManagement.GetUser(userRepository, UserManagement.LoggedInUserName);
            if (user != null && user.Image != null)
            {
                signatureImage = user.Image;
            }

            IRepository<Technician> technicianRepository = ContainerBootstrapper.Resolve<IRepository<Technician>>();
            var technicianUser = technicianRepository.FirstOrDefault(c => string.Equals(c.Name, tachographDocument.Technician));
            if (technicianUser != null && technicianUser.Image != null)
            {
                signatureImage = technicianUser.Image;
            }

            if (signatureImage != null)
            {
                Image image = ImageHelper.Scale(signatureImage, 500, 50);
                document.AddImage(ImageHelper.ToByteArray(image), image.Width, image.Height, x, y);
            }
        }
    }
}