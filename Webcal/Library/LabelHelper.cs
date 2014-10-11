namespace Webcal.Library
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Printing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows.Resources;
    using Core;
    using DataModel;
    using DataModel.Core;
    using Properties;
    using Shared;
    using PrinterSettings = DataModel.PrinterSettings;

    public class LabelHelper : PrintDocument
    {
        private const int LEFT_OFFSET = -225;
        private const int LINE_WIDTH = 200;
        private const int LINE_HEIGHT = 43;

        private const int TEXT_HEIGHT = 43;
        private const int TEXT_SECONDARY_LEFT_OFFSET = 5;

        private static List<int> _trackedPositions;
        private Font _font;
        private int _horizontalLinesVerticalOffset = -160;
        private Font _largeFont;
        private RegistrationData _registrationData;
        private int _textVerticalOffset = -147;
        private WorkshopSettings _workshopSettings;

        public Font Font
        {
            get { return _font ?? (_font = GetFont(15, false)); }
        }

        public Font BoldFont
        {
            get { return _font ?? (_font = GetFont(15, true)); }
        }

        public Font LargeFont
        {
            get { return _largeFont ?? (_largeFont = GetFont(16, false)); }
        }

        public Font LargeBoldFont
        {
            get { return _largeFont ?? (_largeFont = GetFont(16, true)); }
        }

        public RegistrationData RegistrationData
        {
            get { return _registrationData ?? (_registrationData = GetRegistrationData()); }
        }

        public WorkshopSettings WorkshopSettings
        {
            get { return _workshopSettings ?? (_workshopSettings = GetWorkshopSettings()); }
        }

        public void Print(TachographDocument document)
        {
            var printerSettingsRepository = ContainerBootstrapper.Container.GetInstance<IPrinterSettingsRepository>();
            PrinterSettings printerSettings = printerSettingsRepository.GetSettings();

            if (!CanPrintLabel(printerSettings) || Font == null)
            {
                return;
            }

            var bitmap = new Bitmap(560, 450);
            Brush brush = Brushes.Black;
            var pen = new Pen(brush);
            _trackedPositions = new List<int>();

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.TranslateTransform(bitmap.Width/2, bitmap.Height/2);
                g.RotateTransform(270);
                g.DrawLine(pen, LEFT_OFFSET, _horizontalLinesVerticalOffset, LINE_WIDTH, _horizontalLinesVerticalOffset);

                for (int i = 0; i < 7; i++)
                {
                    g.DrawLine(pen, LEFT_OFFSET, Track(_horizontalLinesVerticalOffset += LINE_HEIGHT), LINE_WIDTH, GetLast());
                }

                g.DrawLine(pen, 0, _trackedPositions[0], 0, _trackedPositions[2]);
                g.DrawLine(pen, 0, GetLast(), 0, GetLast() + LINE_HEIGHT*3);
                g.DrawImage(GetWorkshopImage(), -225, -260, 93, 92);

                var settingsRepository = ContainerBootstrapper.Container.GetInstance<IGeneralSettingsRepository>();
                WorkshopSettings settings = settingsRepository.GetSettings();

                if (!string.IsNullOrEmpty(settings.WorkshopName))
                {
                    DrawSpacedText(g, string.Format(Resources.TXT_LABEL_CUSTOMER_NAME, settings.WorkshopName), brush, LargeFont, new PointF(-125, -243));
                }
                if (!string.IsNullOrEmpty(settings.PhoneNumber))
                {
                    DrawSpacedText(g, string.Format(Resources.TXT_LABEL_CUSTOMER_PHONE_NUMBER, settings.PhoneNumber), brush, new PointF(-125, -200));
                }

                DrawSpacedText(g, GetDocumentType(document.DocumentType), brush, new PointF(LEFT_OFFSET, _textVerticalOffset));

                DrawSpacedText(g, string.Format(Resources.TXT_LABEL_K_FACTOR, document.KFactor), brush, new PointF(LEFT_OFFSET, _textVerticalOffset += TEXT_HEIGHT));
                DrawSpacedText(g, string.Format(Resources.TXT_LABEL_W_FACTOR, document.WFactor), brush, new PointF(TEXT_SECONDARY_LEFT_OFFSET, _textVerticalOffset));

                DrawSpacedText(g, string.Format(Resources.TXT_LABEL_L_FACTOR, document.LFactor), brush, new PointF(LEFT_OFFSET, _textVerticalOffset += TEXT_HEIGHT));
                DrawSpacedText(g, string.Format(Resources.TXT_LABEL_SEAL_NUMBER, RegistrationData.SealNumber), brush, new PointF(TEXT_SECONDARY_LEFT_OFFSET, _textVerticalOffset));

                DrawSpacedText(g, string.Format(Resources.TXT_LABEL_VIN, document.VIN), brush, new PointF(LEFT_OFFSET, _textVerticalOffset += TEXT_HEIGHT));
                DrawSpacedText(g, string.Format(Resources.TXT_LABEL_SERIAL_NUMBER, document.SerialNumber), brush, new PointF(LEFT_OFFSET, _textVerticalOffset += TEXT_HEIGHT));
                DrawSpacedText(g, string.Format(Resources.TXT_LABEL_TYRE_SIZE, document.TyreSize), brush, new PointF(LEFT_OFFSET, _textVerticalOffset += TEXT_HEIGHT));

                DrawSpacedText(g, GetCompanyName(), brush, new PointF(LEFT_OFFSET, _textVerticalOffset += TEXT_HEIGHT));

                if (string.IsNullOrEmpty(RegistrationData.LicenseKey) && !(RegistrationData.ExpirationDate < DateTime.Now.Date))
                {
                    DrawSpacedText(g, Resources.TXT_DEMO, brush, new PointF(LEFT_OFFSET, Track(_textVerticalOffset += TEXT_HEIGHT))); //address 1
                    DrawSpacedText(g, Resources.TXT_DATE, brush, new PointF(TEXT_SECONDARY_LEFT_OFFSET, GetLast()));
                    DrawSpacedText(g, Resources.TXT_DEMO, brush, new PointF(LEFT_OFFSET, Track((_textVerticalOffset += TEXT_HEIGHT) - 15))); //town
                    DrawSpacedText(g, Resources.TXT_DEMO, brush, new PointF(LEFT_OFFSET, (_textVerticalOffset += TEXT_HEIGHT) - 30)); //post code
                }
                else
                {
                    DrawSpacedText(g, WorkshopSettings.Address1 ?? string.Empty, brush, new PointF(LEFT_OFFSET, Track(_textVerticalOffset += TEXT_HEIGHT))); //address 1
                    DrawSpacedText(g, Resources.TXT_DATE, brush, new PointF(TEXT_SECONDARY_LEFT_OFFSET, GetLast()));
                    DrawSpacedText(g, WorkshopSettings.Town ?? string.Empty, brush, new PointF(LEFT_OFFSET, Track((_textVerticalOffset += TEXT_HEIGHT) - 15))); //town
                    DrawSpacedText(g, WorkshopSettings.PostCode ?? string.Empty, brush, new PointF(LEFT_OFFSET, (_textVerticalOffset += TEXT_HEIGHT) - 30)); //post code
                }

                DrawSpacedText(g, GetCalibrationTime(document.CalibrationTime), brush, new PointF(TEXT_SECONDARY_LEFT_OFFSET, GetLast()));

                g.Flush();
            }

            bitmap.Save(Path.Combine(DocumentHelper.GetTemporaryDirectory(), "label.bmp"));
            PrinterSettings.PrinterName = printerSettings.DefaultLabelPrinter;
            PrinterSettings.Copies = (short) printerSettings.LabelNumberOfCopies;

            Print();
        }

        public bool CanPrint()
        {
            return WorkshopSettings.AutoPrintLabels;
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            base.OnPrintPage(e);

            using (Image image = ImageHelper.LoadImageSafely(Path.Combine(DocumentHelper.GetTemporaryDirectory(), "label.bmp")))
            {
                e.Graphics.DrawImage(image, new Rectangle(-10, -5, 280, 225));
            }
        }

        private static int Track(int v)
        {
            _trackedPositions.Add(v);
            return v;
        }

        private static int GetLast()
        {
            return _trackedPositions.Last();
        }

        private static string GetCalibrationTime(DateTime? calibrationTime)
        {
            if (calibrationTime == null)
            {
                return string.Empty;
            }

            return calibrationTime.Value.ToString(Constants.DateFormat);
        }

        private void DrawSpacedText(Graphics g, string text, Brush brush, PointF point)
        {
            DrawSpacedText(g, text, brush, Font, point);
        }

        private void DrawSpacedText(Graphics g, string text, Brush brush, Font font, PointF point)
        {
            const float spacing = -6;

            //draw text
            float indent = 0;

            foreach (var character in text)
            {
                if (character == ' ')
                {
                    indent += 5;
                    continue;
                }

                g.DrawString(character.ToString(CultureInfo.InvariantCulture), font, brush, new PointF(point.X + indent, point.Y));
                indent += g.MeasureString(character.ToString(CultureInfo.InvariantCulture), font).Width + spacing;
            }
        }

        private static bool CanPrintLabel(PrinterSettings printerSettings)
        {
            return !string.IsNullOrEmpty(printerSettings.DefaultLabelPrinter);
        }

        private static RegistrationData GetRegistrationData()
        {
            return ContainerBootstrapper.Container.GetInstance<IRepository<RegistrationData>>().GetAll().First();
        }

        private static WorkshopSettings GetWorkshopSettings()
        {
            return ContainerBootstrapper.Container.GetInstance<IGeneralSettingsRepository>().GetSettings();
        }

        private static Font GetFont(int fontSize, bool bold)
        {
            FontFamily fontFamily = FontFamily.Families.FirstOrDefault(t => t.Name == "Lucida Console"); //Really, what are the chances of this going wrong?
            if (fontFamily == null)
            {
                throw new InvalidOperationException(Resources.EXC_UNABLE_FIND_LABEL_FONT);
            }
            if (bold)
            {
                return new Font(fontFamily, fontSize, FontStyle.Bold);
            }
            return new Font(fontFamily, fontSize, FontStyle.Regular);
        }

        private static Image GetWorkshopImage()
        {
            StreamResourceInfo resourceStream = DocumentHelper.GetResourceStreamFromSimplePath("Images/PDF/skillray-tacho-icon.png");
            return Image.FromStream(resourceStream.Stream);
        }

        private string GetCompanyName()
        {
            string companyName = RegistrationData.CompanyName;

            if (string.IsNullOrEmpty(RegistrationData.LicenseKey) && !(RegistrationData.ExpirationDate < DateTime.Now.Date))
            {
                companyName = Resources.TXT_SKILLRAY;
            }

            return companyName;
        }

        private static string GetDocumentType(string documentType)
        {
            if (string.Equals(documentType, Resources.TXT_INSTALLATION_CALIBRATION))
            {
                return Resources.TXT_LABEL_INITIAL_CALIBRATION;
            }

            if (string.Equals(documentType, Resources.TXT_MINOR_WORK_DETAILS))
            {
                return Resources.TXT_LABEL_MINOR_WORK;
            }

            if (string.Equals(documentType, Resources.TXT_RECALIBRATION))
            {
                return Resources.TXT_LABEL_RECALIBRATION;
            }

            if (string.Equals(documentType, Resources.TXT_TWO_YEAR_INSPECTION))
            {
                return Resources.TXT_LABEL_TWO_YEAR_INSPECTION;
            }

            if (string.Equals(documentType, Resources.TXT_SIX_YEAR_CALIBRATION))
            {
                return Resources.TXT_LABEL_SIX_YEAR_CALIBRATION;
            }

            if (string.Equals(documentType, Resources.TXT_DIGITAL_INITIAL_CALIBRATION))
            {
                return Resources.TXT_LABEL_DIGITAL_INITIAL_CALIBRATION;
            }

            if (string.Equals(documentType, Resources.TXT_DIGITAL_TWO_YEAR_CALIBRATION))
            {
                return Resources.TXT_LABEL_DIGITAL_TWO_YEAR;
            }

            return string.Empty;
        }
    }
}