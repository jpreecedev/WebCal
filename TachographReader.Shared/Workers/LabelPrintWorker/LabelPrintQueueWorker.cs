namespace TachographReader.Shared.Workers.LabelPrintWorker
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Printing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Helpers;
    using Properties;
    using Workers;

    public class LabelPrintQueueWorker : BaseWorker
    {
        public override void Start(IWorkerParameters parameters)
        {
            var labelPrintParameters = new LabelPrintParameters(parameters);

            var labelPrintDocument = new LabelPrintDocument();
            labelPrintDocument.Print(labelPrintParameters);
        }

        private class LabelPrintDocument : PrintDocument
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
            private int _textVerticalOffset = -147;

            private Font Font
            {
                get { return _font ?? (_font = GetFont(15, false)); }
            }

            private Font LargeFont
            {
                get { return _largeFont ?? (_largeFont = GetFont(16, false)); }
            }

            private LabelPrintParameters PrintParameters { get; set; }

            public void Print(LabelPrintParameters parameters)
            {
                if (parameters == null)
                {
                    throw new ArgumentNullException(nameof(parameters));
                }

                PrintParameters = parameters;
                if (!CanPrintLabel() || Font == null)
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

                    g.TranslateTransform(bitmap.Width / 2, bitmap.Height / 2);
                    g.RotateTransform(270);
                    g.DrawLine(pen, LEFT_OFFSET, _horizontalLinesVerticalOffset, LINE_WIDTH, _horizontalLinesVerticalOffset);

                    for (int i = 0; i < 7; i++)
                    {
                        Track(_horizontalLinesVerticalOffset += LINE_HEIGHT);
                    }

                    g.DrawImage(GetWorkshopImage(), -225, -260, 93, 92);

                    if (!string.IsNullOrEmpty(PrintParameters.WorkshopName))
                    {
                        DrawSpacedText(g, string.Format(Resources.TXT_LABEL_CUSTOMER_NAME, PrintParameters.WorkshopName), brush, LargeFont, new PointF(-125, -243));
                    }
                    if (!string.IsNullOrEmpty(PrintParameters.PhoneNumber))
                    {
                        DrawSpacedText(g, string.Format(Resources.TXT_LABEL_CUSTOMER_PHONE_NUMBER, PrintParameters.PhoneNumber), brush, new PointF(-125, -200));
                    }

                    DrawCenteredText(g, GetDocumentType(PrintParameters.DocumentType), brush, new PointF(LEFT_OFFSET, _textVerticalOffset));

                    DrawSpacedText(g, string.Format(Resources.TXT_LABEL_K_FACTOR, PrintParameters.KFactor), brush, new PointF(LEFT_OFFSET, _textVerticalOffset += TEXT_HEIGHT));

                    var wFactorVerticalOffset = _textVerticalOffset += TEXT_HEIGHT;
                    DrawSpacedText(g, string.Format(Resources.TXT_LABEL_W_FACTOR, PrintParameters.WFactor), brush, new PointF(LEFT_OFFSET, wFactorVerticalOffset));
                    
                    DrawSpacedText(g, Resources.TXT_CALIBRATION_DATE, brush, new PointF(TEXT_SECONDARY_LEFT_OFFSET, wFactorVerticalOffset));
                    DrawSpacedText(g, GetCalibrationTime(PrintParameters.CalibrationTime), brush, new PointF(TEXT_SECONDARY_LEFT_OFFSET, wFactorVerticalOffset + TEXT_HEIGHT));
                    DrawRectangle(g, TEXT_SECONDARY_LEFT_OFFSET, wFactorVerticalOffset, 130, 70);

                    DrawSpacedText(g, string.Format(Resources.TXT_LABEL_L_FACTOR, PrintParameters.LFactor), brush, new PointF(LEFT_OFFSET, _textVerticalOffset += TEXT_HEIGHT));
                    DrawSpacedText(g, string.Format(Resources.TXT_LABEL_VIN, PrintParameters.VIN), brush, new PointF(LEFT_OFFSET, _textVerticalOffset += TEXT_HEIGHT));
                    DrawSpacedText(g, string.Format(Resources.TXT_LABEL_SERIAL_NUMBER, PrintParameters.SerialNumber), brush, new PointF(LEFT_OFFSET, _textVerticalOffset += TEXT_HEIGHT));
                    DrawSpacedText(g, string.Format(Resources.TXT_LABEL_SEAL_NUMBER, PrintParameters.SealNumber), brush, new PointF(TEXT_SECONDARY_LEFT_OFFSET, _textVerticalOffset));
                    DrawSpacedText(g, string.Format(Resources.TXT_LABEL_TYRE_SIZE, PrintParameters.TyreSize), brush, new PointF(LEFT_OFFSET, _textVerticalOffset += TEXT_HEIGHT));

                    DrawSpacedText(g, GetCompanyName(), brush, new PointF(LEFT_OFFSET, _textVerticalOffset += TEXT_HEIGHT));

                    if (!string.IsNullOrEmpty(PrintParameters.LicenseKey) && PrintParameters.ExpirationDate >= DateTime.Now.Date)
                    {
                        DrawSpacedText(g, PrintParameters.Address1 ?? string.Empty, brush, new PointF(LEFT_OFFSET, Track(_textVerticalOffset += TEXT_HEIGHT))); //address 1
                        DrawSpacedText(g, PrintParameters.Town ?? string.Empty, brush, new PointF(LEFT_OFFSET, Track((_textVerticalOffset += TEXT_HEIGHT) - 15))); //town
                        DrawSpacedText(g, PrintParameters.PostCode ?? string.Empty, brush, new PointF(LEFT_OFFSET, (_textVerticalOffset += TEXT_HEIGHT) - 30)); //post code
                    }
                    else
                    {
                        DrawSpacedText(g, Resources.TXT_DEMO, brush, new PointF(LEFT_OFFSET, Track(_textVerticalOffset += TEXT_HEIGHT))); //address 1
                        DrawSpacedText(g, Resources.TXT_DEMO, brush, new PointF(LEFT_OFFSET, Track((_textVerticalOffset += TEXT_HEIGHT) - 15))); //town
                        DrawSpacedText(g, Resources.TXT_DEMO, brush, new PointF(LEFT_OFFSET, (_textVerticalOffset += TEXT_HEIGHT) - 30)); //post code
                    }

                    g.Flush();
                }

                bitmap.Save(Path.Combine(PrintParameters.TemporaryDirectory, "label.bmp"));
                PrinterSettings.PrinterName = PrintParameters.DefaultLabelPrinter;
                PrinterSettings.Copies = (short)PrintParameters.LabelNumberOfCopies;

                Print();
            }

            protected override void OnPrintPage(PrintPageEventArgs e)
            {
                base.OnPrintPage(e);

                using (Image image = ImageHelper.LoadImageSafely(Path.Combine(PrintParameters.TemporaryDirectory, "label.bmp")))
                {
                    e.Graphics.DrawImage(image, new Rectangle(-10, -5, 280, 225));
                }
            }

            private static int Track(int v)
            {
                _trackedPositions.Add(v);
                return v;
            }

            private string GetCalibrationTime(DateTime? calibrationTime)
            {
                if (calibrationTime == null)
                {
                    return string.Empty;
                }

                return calibrationTime.Value.ToString(PrintParameters.DateFormat);
            }

            private void DrawCenteredText(Graphics g, string text, Brush brush, PointF point)
            {
                var stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center
                };

                g.DrawString(text, Font, brush, new RectangleF(point, new SizeF(425,30)), stringFormat);
            }

            private void DrawRectangle(Graphics g, int x, int y, int width, int height)
            {
                g.DrawRectangle(Pens.Black, new Rectangle(x, y, width, height));
            }

            private void DrawSpacedText(Graphics g, string text, Brush brush, PointF point)
            {
                DrawSpacedText(g, text, brush, Font, point, null);
            }

            private static void DrawSpacedText(Graphics g, string text, Brush brush, Font font, PointF point)
            {
                DrawSpacedText(g, text, brush, font, point, null);
            }

            private static void DrawSpacedText(Graphics g, string text, Brush brush, Font font, PointF point, StringFormat stringFormat)
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

                    g.DrawString(character.ToString(CultureInfo.InvariantCulture), font, brush, new PointF(point.X + indent, point.Y), stringFormat);
                    indent += g.MeasureString(character.ToString(CultureInfo.InvariantCulture), font).Width + spacing;
                }
            }

            private bool CanPrintLabel()
            {
                return !string.IsNullOrEmpty(PrintParameters.DefaultLabelPrinter);
            }

            private Font GetFont(int fontSize, bool bold)
            {
                FontFamily fontFamily = FontFamily.Families.FirstOrDefault(t => t.Name == PrintParameters.DefaultFont);
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

            private Image GetWorkshopImage()
            {
                return PrintParameters.SkillrayTachoIcon;
            }

            private string GetCompanyName()
            {
                string companyName = PrintParameters.CompanyName;

                if (string.IsNullOrEmpty(PrintParameters.LicenseKey) || PrintParameters.ExpirationDate < DateTime.Now.Date)
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
}