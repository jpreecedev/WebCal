namespace Webcal.Library.PDF
{
    using System;
    using System.Drawing;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using Org.BouncyCastle.Utilities.Collections;
    using Properties;
    using Shared;
    using Image = System.Drawing.Image;

    public class MinimalPlaqueDocument : BasePlaqueDocument
    {
        protected override string GetTitle()
        {
            return Resources.TXT_TACHOGRAPH_PLAQUE;
        }

        protected override void CreateLargeLabelLogos(PDFDocument document, TachographDocument tachographDocument, int startHorizontal, int startVertical)
        {
            if (WorkshopSettings.Image != null)
            {
                const float profileImageMaxHeight = 145;
                const float profileImageMaxWidth = 545;

                float widthScale = profileImageMaxWidth / WorkshopSettings.Image.Width;
                float heightScale = profileImageMaxHeight / WorkshopSettings.Image.Height;
                float scale = Math.Min(widthScale, heightScale);
                float newWidth = WorkshopSettings.Image.Width * scale;
                float newHeight = WorkshopSettings.Image.Height * scale;

                document.AddImage(WorkshopSettings.RawImage, newWidth, newHeight, (startHorizontal + 20), (startVertical + 675));
            }

            //Skillray
            Image image = Image.FromStream(DocumentHelper.GetResourceStreamFromSimplePath("../Images/webcal-print-logo.jpg").Stream);
            document.AddImage(image.ToByteArray(), 180, 41, (startHorizontal + 350), (startVertical + 670));
        }

        protected override void CreateLargeLabelAddress(PDFDocument document, TachographDocument tachographDocument, int startHorizontal, int startVertical)
        {
            //Add Workshop Adress!!    
            int verticalPos = startVertical;
            if (CustomerContact != null)
            {
                if (CustomerContact.Name != string.Empty)
                {
                    AbsolutePositionText(document, CustomerContact.Name, (startHorizontal + 350), (verticalPos), 580, 100, document.GetXLargeFont(false));
                    verticalPos += 20;
                }
                if (CustomerContact.Address != string.Empty)
                {
                    AbsolutePositionText(document, CustomerContact.Address, (startHorizontal + 350), (verticalPos), 580, 100, document.GetXLargeFont(false));
                    verticalPos += 20;
                }
                if (CustomerContact.Town != string.Empty)
                {
                    AbsolutePositionText(document, CustomerContact.Town, (startHorizontal + 350), (verticalPos), 580, 100, document.GetXLargeFont(false));
                    verticalPos += 20;
                }
                if (CustomerContact.PostCode != string.Empty)
                {
                    AbsolutePositionText(document, CustomerContact.PostCode, (startHorizontal + 350), (verticalPos), 580, 100, document.GetXLargeFont(false));
                }
            }


            //Add Company Address
            verticalPos = startVertical;
            if (WorkshopSettings.WorkshopName != string.Empty)
            {
                AbsolutePositionText(document, WorkshopSettings.WorkshopName, (startHorizontal + 32), (verticalPos), 580, 100, document.GetXLargeFont(false));
                verticalPos += 20;
            }
            if (WorkshopSettings.Address1 != string.Empty)
            {
                AbsolutePositionText(document, WorkshopSettings.Address1, (startHorizontal + 32), (verticalPos), 580, 100, document.GetXLargeFont(false));
                verticalPos += 20;
            }

            if (WorkshopSettings.Town != string.Empty)
            {
                AbsolutePositionText(document, WorkshopSettings.Town, (startHorizontal + 32), (verticalPos), 580, 100, document.GetXLargeFont(false));
                verticalPos += 20;
            }

            if (WorkshopSettings.PostCode != string.Empty)
            {
                AbsolutePositionText(document, WorkshopSettings.PostCode, (startHorizontal + 32), (verticalPos), 580, 100, document.GetXLargeFont(false));
            }
        }

        protected override void CreateLargeLabelExpiry(PDFDocument document, TachographDocument tachographDocument, int startHorizontal, int startVertical)
        {
            DateTime? calibrationDate = tachographDocument.CalibrationTime ?? DateTime.Today;

            string expiryDate = Resources.TXT_EXPIRATION_DATE + (GetCalibrationTime(calibrationDate.Value.AddYears(2).AddDays(-1)));

            AbsolutePositionText(document, expiryDate, (startHorizontal + 30), (startVertical), 580, 100, document.GetXLargeFont(false));
        }

        protected override void CreateLargeLabel(PDFDocument document, TachographDocument tachographDocument, int startHorizontal, int startVertical)
        {
            AbsolutePositionText(document, Resources.TXT_TACHOGRAPH_RECORD_SHEET, (startHorizontal + 32), (startVertical + 0), 580, 100, document.GetXLargeFont(false), Element.ALIGN_CENTER);
            AbsolutePositionText(document, TrimDocumentType(tachographDocument.DocumentType), (startHorizontal + 32), (startVertical + 30), 580, 100, document.GetRegularFont(false));

            //Outlining
            DrawLargeLabelRectangle(document, startHorizontal, startVertical);
            document.DrawLine((startHorizontal + 50), (startVertical + 146), (startHorizontal + 545), (startVertical + 146), TotalPageHeight);
            document.DrawLine((startHorizontal + 50), (startVertical + 234), (startHorizontal + 545), (startVertical + 234), TotalPageHeight);
            document.DrawLine((startHorizontal + 150), (startVertical + 334), (startHorizontal + 298), (startVertical + 334), TotalPageHeight);
            document.DrawLine((startHorizontal + 347), (startVertical + 334), (startHorizontal + 494), (startVertical + 334), TotalPageHeight);

            AbsolutePositionText(document, Resources.TXT_TACHOGRAPH, (startHorizontal + 50), (startVertical + 93), 150, 100);
            AbsolutePositionText(document, Resources.TXT_VEHICLE, (startHorizontal + 50), (startVertical + 175), 150, 100);
            AbsolutePositionText(document, Resources.TXT_TECHNICIAN, (startHorizontal + 50), (startVertical + 270), 150, 20);
            AbsolutePositionText(document, Resources.TXT_DATE, (startHorizontal + 150), (startVertical + 328), 200, 40);

            TryAddSignature(document, tachographDocument, (startHorizontal + 330));
            AbsolutePositionText(document, string.Format(Resources.TXT_SIGNATURE_TECHNICIAN_LARGE, tachographDocument.Technician), (startHorizontal + 347), (startVertical + 328), 550, 40);

            //Tachograph
            AbsolutePositionText(document, Resources.TXT_MAKE, (startHorizontal + 150), (startVertical + 60), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, Resources.TXT_TYPE, (startHorizontal + 347), (startVertical + 60), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, tachographDocument.TachographMake, (startHorizontal + 150), (startVertical + 72), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, tachographDocument.TachographModel, (startHorizontal + 347), (startVertical + 72), 550, 72, document.GetRegularFont(false));

            AbsolutePositionText(document, Resources.TXT_ODOMETER_READING, (startHorizontal + 150), (startVertical + 86), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, Resources.TXT_SERIAL_NUMBER, (startHorizontal + 347), (startVertical + 86), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, tachographDocument.OdometerReading, (startHorizontal + 150), (startVertical + 98), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, tachographDocument.SerialNumber, (startHorizontal + 347), (startVertical + 98), 550, 72, document.GetRegularFont(false));

            AbsolutePositionText(document, Resources.TXT_SET_K_FACTOR, (startHorizontal + 150), (startVertical + 112), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, tachographDocument.KFactor, (startHorizontal + 150), (startVertical + 124), 550, 72, document.GetRegularFont(false));

            if (tachographDocument.DocumentType == Resources.TXT_MINOR_WORK_DETAILS)
            {
                const int workDetailsHeight = 19;
                document.ContentByte.SetColorStroke(new BaseColor(Color.Black));
                document.ContentByte.Rectangle((startHorizontal + 20), (TotalPageHeight - (startVertical + 44 + workDetailsHeight)), 555, workDetailsHeight);
                document.ContentByte.Stroke();

                AbsolutePositionText(document, Resources.TXT_WORK_CARRIED_OUT, (startHorizontal + 50), (startVertical + 40), 150, 100, document.GetSmallerFont());
                AbsolutePositionText(document, tachographDocument.MinorWorkDetails, (startHorizontal + 150), (startVertical + 40), 450, 100, document.GetSmallerFont());
            }

            //Vehicle

            AbsolutePositionText(document, Resources.TXT_MAKE_AND_MODEL, (startHorizontal + 150), (startVertical + 144), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, Resources.TXT_VRN, (startHorizontal + 347), (startVertical + 144), 550, 72, document.GetRegularFont(false));

            AbsolutePositionText(document, string.Format("{0} {1}", tachographDocument.VehicleMake, tachographDocument.VehicleModel), (startHorizontal + 150), (startVertical + 156), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, tachographDocument.RegistrationNumber, (startHorizontal + 347), (startVertical + 156), 550, 72, document.GetRegularFont(false));

            AbsolutePositionText(document, Resources.TXT_VIN, (startHorizontal + 150), (startVertical + 170), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, Resources.TXT_TYRE_SIZE, (startHorizontal + 347), (startVertical + 170), 550, 72, document.GetRegularFont(false));

            AbsolutePositionText(document, tachographDocument.VIN, (startHorizontal + 150), (startVertical + 182), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, tachographDocument.TyreSize, (startHorizontal + 347), (startVertical + 182), 550, 72, document.GetRegularFont(false));

            AbsolutePositionText(document, Resources.TXT_W_FACTOR, (startHorizontal + 150), (startVertical + 196), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, Resources.TXT_TYRE_L_FACTOR, (startHorizontal + 347), (startVertical + 196), 550, 72, document.GetRegularFont(false));

            AbsolutePositionText(document, string.Format(Resources.TXT_IMP_KM_VEHICLE, tachographDocument.WFactor), (startHorizontal + 150), (startVertical + 208), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, tachographDocument.LFactor + Resources.TXT_MM, (startHorizontal + 347), (startVertical + 208), 550, 72, document.GetRegularFont(false));

            //Technician

            AbsolutePositionText(document, RegistrationData.CompanyName, (startHorizontal + 150), (startVertical + 234), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, WorkshopSettings.Address1, (startHorizontal + 150), (startVertical + 258), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, string.Format("{0} {1}", WorkshopSettings.Town, WorkshopSettings.PostCode), (startHorizontal + 150), (startVertical + 272), 550, 72, document.GetRegularFont(false));

            AbsolutePositionText(document, string.Format(Resources.TXT_MINIMAL_LICENSE_NUMBER, RegistrationData.SealNumber), (startHorizontal + 347), (startVertical + 234), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, GetCalibrationTime(tachographDocument.CalibrationTime), (startHorizontal + 150), (startVertical + 314), 250, 40, document.GetRegularFont(false));
        }

        protected override void DrawLargeLabelRectangle(PDFDocument document, int startHorizontal, int startVertical)
        {
            const int height = 283;

            document.ContentByte.SetColorStroke(new BaseColor(Color.Black));
            document.ContentByte.Rectangle((startHorizontal + 20), (TotalPageHeight - (startVertical + 66 + height)), 555, height);
            document.ContentByte.Stroke();
        }

        protected override void CreateKFactorLabel(PDFDocument document, TachographDocument tachographDocument)
        {
            ColumnText kValueColumn = document.GetNewColumn(425, document.Height - 356, 480, 100);
            document.AddParagraph(tachographDocument.KFactor, kValueColumn);
        }

        protected override void CreateMediumLabel(PDFDocument document, TachographDocument tachographDocument)
        {
            var table = new PdfPTable(4) { TotalWidth = 266 };
            table.SetWidths(new float[] { 108, 54, 54, 108 });

            GetWorkshopImage(document, table);
            document.AddSpannedCell(table, TrimDocumentType(tachographDocument.DocumentType), 2, document.GetSmallerFont(), 29);

            document.AddSpannedCell(table, string.Format(Resources.TXT_MEDIUM_W_FACTOR, tachographDocument.WFactor), 1, document.GetXSmallFont(false), 16);
            document.AddSpannedCell(table, string.Format(Resources.TXT_MEDIUM_K_FACTOR, tachographDocument.KFactor), 2, document.GetXSmallFont(false), 16);
            document.AddSpannedCell(table, string.Format(Resources.TXT_MEDIUM_L_FACTOR, tachographDocument.LFactor, Resources.TXT_MM), 1, document.GetXSmallFont(false), 16);

            document.AddSpannedCell(table, string.Format(Resources.TXT_MEDIUM_VIN, tachographDocument.VIN), 2, document.GetSmallFont(false), 16, 68);
            document.AddSpannedCell(table, string.Format(Resources.TXT_MEDIUM_TYRE_SIZE, tachographDocument.TyreSize), 2, document.GetSmallFont(false), 17, Element.ALIGN_LEFT);

            document.AddSpannedCell(table, string.Format(Resources.TXT_MEDIUM_SERIAL_NUMBER, tachographDocument.SerialNumber), 2, document.GetSmallFont(false), 17, Element.ALIGN_LEFT);
            document.AddSpannedCell(table, string.Format(Resources.TXT_MEDIUM_DATE, GetCalibrationTime(tachographDocument.CalibrationTime)), 2, document.GetRegularFont(false));

            GetCompanyDetails(document, table, 45, 4, document.GetRegularFont(false));
            document.AddSpannedCell(table, GetLicenseNumberParagraph(document, true), 4, 12, Element.ALIGN_LEFT);

            table.WriteSelectedRows(0, -1, document.Document.LeftMargin + 193, document.Height - 103, document.ContentByte);
        }

        protected override void CreateSmallLabel(PDFDocument document, TachographDocument tachographDocument)
        {
            var table = new PdfPTable(4) { TotalWidth = 136 };
            table.SetWidths(new float[] { 58, 10, 20, 48 });

            GetSmallImage(document, table);

            document.AddSpannedCell(table, TrimDocumentType(tachographDocument.DocumentType), 4, document.GetSmallerFont(), 21);

            document.AddSpannedCell(table, string.Format(Resources.TXT_SMALL_W_FACTOR, tachographDocument.WFactor), 2, document.GetXSmallFont(false), 20);
            document.AddSpannedCell(table, string.Format(Resources.TXT_SMALL_K_FACTOR, tachographDocument.KFactor), 2, document.GetXSmallFont(false), 20);

            document.AddSpannedCell(table, string.Format(Resources.TXT_SMALL_L_FACTOR, tachographDocument.LFactor, Resources.TXT_MM), 4, document.GetRegularFont(false), 20, 58);
            document.AddSpannedCell(table, string.Format(Resources.TXT_SMALL_TYRE_SIZE, tachographDocument.TyreSize), 4, document.GetSmallerFont(), 20, Element.ALIGN_LEFT);
            document.AddSpannedCell(table, string.Format(Resources.TXT_SMALL_VIN, tachographDocument.VIN), 4, document.GetSmallerFont(), 20, 68);
            document.AddSpannedCell(table, string.Format(Resources.TXT_SMALL_SERIAL_NUMBER, tachographDocument.SerialNumber), 4, document.GetSmallerFont(), 20, Element.ALIGN_LEFT);

            GetCompanyDetails(document, table, 85, 3, document.GetRegularFont(false));

            document.AddSpannedCell(table, string.Format(Resources.TXT_SMALL_DATE, GetCalibrationTime(tachographDocument.CalibrationTime)), 1, document.GetSmallFont(false));
            document.AddSpannedCell(table, GetLicenseNumberParagraph(document, false), 4, 20, Element.ALIGN_LEFT);

            table.WriteSelectedRows(0, -1, document.Document.LeftMargin + 2, document.Height - 103, document.ContentByte);
        }

        protected override void CreateLargeCertificate(PDFDocument document, TachographDocument tachographDocument, bool excludeLogos)
        {
            const int startHorizontal = 20;
            const int startVertical = 0;

            document.DrawLine((startHorizontal), (startVertical), (startHorizontal + 555), (startVertical), TotalPageHeight);

            document.DrawLine((startHorizontal), (startVertical), (startHorizontal), (startVertical + 800), TotalPageHeight);

            document.DrawLine((startHorizontal + 555), (startVertical), (startHorizontal + 555), (startVertical + 800), TotalPageHeight);

            document.DrawLine((startHorizontal), (startVertical + 800), (startHorizontal + 555), (startVertical + 800), TotalPageHeight);

            document.DrawLine((startHorizontal), (startVertical + 290), (startHorizontal + 555), (startVertical + 290), TotalPageHeight);

            if (tachographDocument.MinorWorkDetails == null)
            {
                document.DrawLine((startHorizontal), (startVertical + 400), (startHorizontal + 555), (startVertical + 400), TotalPageHeight);
                document.DrawLine((startHorizontal), (startVertical + 600), (startHorizontal + 555), (startVertical + 600), TotalPageHeight);
            }
            else
            {
                document.DrawLine((startHorizontal), (startVertical + 420), (startHorizontal + 555), (startVertical + 420), TotalPageHeight);
                document.DrawLine((startHorizontal), (startVertical + 630), (startHorizontal + 555), (startVertical + 630), TotalPageHeight);
            }

            if (WorkshopSettings.Image != null && !excludeLogos)
            {
                var image = ImageHelper.Scale(WorkshopSettings.Image, 150);
                document.AddImage(image.ToByteArray(), image.Width, image.Height, (startHorizontal + 5), startVertical + 660);
            }

            AbsolutePositionText(document, Resources.TXT_TACHOGRAPH_CALIBRATION_CERTIFICATE.ToUpper(), (startHorizontal + 200), (startVertical + 170), 580, 100, document.GetRegularFont(true));
            document.DrawLine((startHorizontal + 200), (startVertical + 188), (startHorizontal + 367), (startVertical + 188), TotalPageHeight);

            if (CustomerContact != null)
            {
                AbsolutePositionText(document, Resources.TXT_CUSTOMER_DETAILS.ToUpper(), (startHorizontal + 5), (startVertical + 200), 200, 40, document.GetRegularFont(true));
                if (CustomerContact.Name != null)
                {
                    AbsolutePositionText(document, CustomerContact.Name.ToUpper(), (startHorizontal + 5), (startVertical + 220), 200, 40, document.GetRegularFont(false));
                }
                if (CustomerContact.Address != null)
                {
                    AbsolutePositionText(document, CustomerContact.Address.ToUpper(), (startHorizontal + 5), (startVertical + 230), 200, 40, document.GetRegularFont(false));
                }
                if (CustomerContact.Town != null)
                {
                    AbsolutePositionText(document, CustomerContact.Town.ToUpper(), (startHorizontal + 5), (startVertical + 240), 200, 40, document.GetRegularFont(false));
                }
                if (CustomerContact.PostCode != null)
                {
                    AbsolutePositionText(document, CustomerContact.PostCode.ToUpper(), (startHorizontal + 5), (startVertical + 250), 200, 40, document.GetRegularFont(false));
                }
            }


            DateTime? calibrationDate = tachographDocument.CalibrationTime;

            if (calibrationDate == null)
            {
                calibrationDate = DateTime.Today;
            }

            string expiryDate = (GetCalibrationTime(calibrationDate.Value.AddYears(2).AddDays(-1)));

            AbsolutePositionText(document, Resources.TXT_DATE.ToUpper(), (startHorizontal + 370), (startVertical + 210), 355, 40, document.GetRegularFont(true));
            AbsolutePositionText(document, Resources.TXT_EXP.ToUpper(), (startHorizontal + 260), (startVertical + 220), 400, 40, document.GetRegularFont(true));

            AbsolutePositionText(document, GetCalibrationTime(tachographDocument.CalibrationTime).ToUpper(), (startHorizontal + 450), (startVertical + 210), 390, 40, document.GetRegularFont(false));
            AbsolutePositionText(document, expiryDate.ToUpper(), (startHorizontal + 450), (startVertical + 220), 390, 40, document.GetRegularFont(false));

            AbsolutePositionText(document, Resources.TXT_ANALOGUE_INSPECTIONS.ToUpper(), (startHorizontal + 5), (startVertical + 300), 590, 40, document.GetRegularFont(true));

            AbsolutePositionText(document, Resources.TXT_CALIBRATION.ToUpper(), (startHorizontal + 15), (startVertical + 310), 590, 40, document.GetRegularFont(false));
            document.DrawCheckBox((startHorizontal + 5), (startVertical + 320), TotalPageHeight);
            if (tachographDocument.DocumentType == Resources.TXT_LARGE_INSTALLATION)
            {
                document.DrawCheck((startHorizontal + 5), (startVertical + 320), TotalPageHeight);
            }

            AbsolutePositionText(document, Resources.TXT_2_YEAR.ToUpper(), (startHorizontal + 15), (startVertical + 320), 590, 40, document.GetRegularFont(false));
            document.DrawCheckBox((startHorizontal + 5), (startVertical + 330), TotalPageHeight);
            if (tachographDocument.DocumentType == Resources.TXT_LARGE_TWO_YEAR_INSPECTION)
            {
                document.DrawCheck((startHorizontal + 5), (startVertical + 330), TotalPageHeight);
            }

            AbsolutePositionText(document, Resources.TXT_6_YEAR.ToUpper(), (startHorizontal + 15), (startVertical + 330), 590, 40, document.GetRegularFont(false));
            document.DrawCheckBox((startHorizontal + 5), (startVertical + 340), TotalPageHeight);
            if (tachographDocument.DocumentType == Resources.TXT_LARGE_SIX_YEAR_CALIBRATION)
            {
                document.DrawCheck((startHorizontal + 5), (startVertical + 340), TotalPageHeight);
            }

            AbsolutePositionText(document, Resources.TXT_RECALIBRATION_CERTIFICATE.ToUpper(), (startHorizontal + 15), (startVertical + 340), 590, 40, document.GetRegularFont(false));
            document.DrawCheckBox((startHorizontal + 5), (startVertical + 350), TotalPageHeight);
            if (tachographDocument.DocumentType == Resources.TXT_LARGE_RECALIBRATION)
            {
                document.DrawCheck((startHorizontal + 5), (startVertical + 350), TotalPageHeight);
            }

            AbsolutePositionText(document, Resources.TXT_MINOR_WORK.ToUpper(), (startHorizontal + 15),
                (startVertical + 350), 590, 40, document.GetRegularFont(false));
            document.DrawCheckBox((startHorizontal + 5), (startVertical + 360), TotalPageHeight);
            if (tachographDocument.DocumentType == Resources.TXT_LARGE_MINOR_WORK && !tachographDocument.IsDigital)
            {
                document.DrawCheck((startHorizontal + 5), (startVertical + 360), TotalPageHeight);
            }

            AbsolutePositionText(document, Resources.TXT_DIGITAL_INSPECTIONS.ToUpper(), (startHorizontal + 240), (startVertical + 300), 590, 40, document.GetRegularFont(true));

            AbsolutePositionText(document, Resources.TXT_CALIBRATION.ToUpper(), (startHorizontal + 255), (startVertical + 310), 590, 40, document.GetRegularFont(false));
            document.DrawCheckBox((startHorizontal + 240), (startVertical + 320), TotalPageHeight);
            if (tachographDocument.DocumentType == Resources.TXT_LARGE_DIGITAL_INITIAL)
            {
                document.DrawCheck((startHorizontal + 240), (startVertical + 320), TotalPageHeight);
            }

            AbsolutePositionText(document, Resources.TXT_2_YEAR.ToUpper(), (startHorizontal + 255),
                (startVertical + 320), 590, 40, document.GetRegularFont(false));
            document.DrawCheckBox((startHorizontal + 240), (startVertical + 330), TotalPageHeight);
            if (tachographDocument.DocumentType == Resources.TXT_LARGE_DIGITAL_TWO_YEAR)
            {
                document.DrawCheck((startHorizontal + 240), (startVertical + 330), TotalPageHeight);
            }

            AbsolutePositionText(document, Resources.TXT_MINOR_WORK.ToUpper(), (startHorizontal + 255), (startVertical + 330), 590, 40, document.GetRegularFont(false));
            document.DrawCheckBox((startHorizontal + 240), (startVertical + 340), TotalPageHeight);
            if (tachographDocument.DocumentType == Resources.TXT_LARGE_MINOR_WORK && tachographDocument.IsDigital)
            {
                document.DrawCheck((startHorizontal + 240), (startVertical + 340), TotalPageHeight);
            }

            if (tachographDocument.MinorWorkDetails != null)
            {
                AbsolutePositionText(document, Resources.TXT_MINOR_WORK_DETAILS_DETAILS.ToUpper(), (startHorizontal + 5), (startVertical + 370), 550, 72, document.GetRegularFont(true));
                AbsolutePositionText(document, tachographDocument.MinorWorkDetails.ToUpper(), (startHorizontal + 5), (startVertical + 380), 550, 72);
            }

            if (tachographDocument.MinorWorkDetails == null)
            {
                AbsolutePositionText(document, Resources.TXT_TACHOGRAPH.ToUpper(), (startHorizontal + 5), (startVertical + 410), 550, 72, document.GetRegularFont(true));
                AbsolutePositionText(document, Resources.TXT_MAKE.ToUpper(), (startHorizontal + 140), (startVertical + 430), 550, 72, document.GetRegularFont(true));
                AbsolutePositionText(document, Resources.TXT_TYPE.ToUpper(), (startHorizontal + 140), (startVertical + 440), 550, 72, document.GetRegularFont(true));
                AbsolutePositionText(document, Resources.TXT_SERIAL_NUMBER.ToUpper(), (startHorizontal + 140), (startVertical + 450), 550, 72, document.GetRegularFont(true));

                AbsolutePositionText(document, tachographDocument.TachographMake.ToUpper(), (startHorizontal + 300), (startVertical + 430), 550, 72);
                AbsolutePositionText(document, tachographDocument.TachographModel.ToUpper(), (startHorizontal + 300), (startVertical + 440), 550, 72);
                AbsolutePositionText(document, tachographDocument.SerialNumber.ToUpper(), (startHorizontal + 300), (startVertical + 450), 550, 72);

                AbsolutePositionText(document, Resources.TXT_VEHICLE.ToUpper(), (startHorizontal + 5), (startVertical + 470), 550, 72, document.GetRegularFont(true));

                AbsolutePositionText(document, Resources.TXT_VRN.ToUpper(), (startHorizontal + 140), (startVertical + 490), 550, 72, document.GetRegularFont(true));
                AbsolutePositionText(document, Resources.TXT_VIN.ToUpper(), (startHorizontal + 140), (startVertical + 500), 550, 72, document.GetRegularFont(true));
                AbsolutePositionText(document, Resources.TXT_TYRE_SIZE.ToUpper(), (startHorizontal + 140), (startVertical + 510), 550, 72, document.GetRegularFont(true));
                AbsolutePositionText(document, Resources.TXT_W_FACTOR.ToUpper(), (startHorizontal + 140), (startVertical + 520), 550, 72, document.GetRegularFont(true));
                AbsolutePositionText(document, Resources.TXT_TYRE_L_FACTOR.ToUpper(), (startHorizontal + 140), (startVertical + 530), 550, 72, document.GetRegularFont(true));
                AbsolutePositionText(document, Resources.TXT_ODOMETER_READING.ToUpper(), (startHorizontal + 140), (startVertical + 540), 550, 72, document.GetRegularFont(true));
                AbsolutePositionText(document, Resources.TXT_SET_K_FACTOR.ToUpper(), (startHorizontal + 140), (startVertical + 550), 550, 72, document.GetRegularFont(true));

                AbsolutePositionText(document, string.Format("{0} {1}", tachographDocument.VehicleMake, tachographDocument.VehicleModel).ToUpper(), (startHorizontal + 140), (startVertical + 480), 550, 72, document.GetRegularFont(true));

                AbsolutePositionText(document, tachographDocument.RegistrationNumber.ToUpper(), (startHorizontal + 300), (startVertical + 490), 550, 72);
                AbsolutePositionText(document, tachographDocument.VIN.ToUpper(), (startHorizontal + 300), (startVertical + 500), 550, 72);
                AbsolutePositionText(document, tachographDocument.TyreSize.ToUpper(), (startHorizontal + 300), (startVertical + 510), 550, 72);
                AbsolutePositionText(document, string.Format(Resources.TXT_IMP_KM, tachographDocument.WFactor).ToUpper(), (startHorizontal + 300), (startVertical + 520), 550, 72);
                AbsolutePositionText(document, string.Format(Resources.TXT_MM, tachographDocument.LFactor).ToUpper(), (startHorizontal + 300), (startVertical + 530), 550, 72);
                AbsolutePositionText(document, tachographDocument.OdometerReading.ToUpper(), (startHorizontal + 300), (startVertical + 540), 550, 72);
                AbsolutePositionText(document, string.Format(Resources.TXT_IMP_KM, tachographDocument.KFactor).ToUpper(), (startHorizontal + 300), (startVertical + 550), 550, 72);

                if (tachographDocument.MinorWorkDetails != null)
                {
                    AbsolutePositionText(document, Resources.TXT_MINOR_WORK_DETAILS_DETAILS.ToUpper(), (startHorizontal + 140), (startVertical + 570), 550, 72, document.GetRegularFont(true));
                    AbsolutePositionText(document, tachographDocument.MinorWorkDetails.ToUpper(), (startHorizontal + 140), (startVertical + 580), 550, 72);
                }

                AbsolutePositionText(document, Resources.TXT_AUTHORISED_TACHOGRAPH_CENTER, (startHorizontal + 30), (startVertical + 620), 550, 72, document.GetRegularFont(true));

                AbsolutePositionText(document, RegistrationData.CompanyName, (startHorizontal + 30), (startVertical + 650), 550, 72, document.GetRegularFont(false));

                AbsolutePositionText(document, WorkshopSettings.Address1, (startHorizontal + 30), (startVertical + 670), 550, 72, document.GetRegularFont(false));

                AbsolutePositionText(document, WorkshopSettings.Town, (startHorizontal + 30), (startVertical + 690), 550, 72, document.GetRegularFont(false));

                AbsolutePositionText(document, WorkshopSettings.PostCode, (startHorizontal + 30), (startVertical + 710), 550, 72, document.GetRegularFont(false));

                AbsolutePositionText(document, Resources.TXT_AUTHORISED_TESTERS_SIGNATURE, (startHorizontal + 300), (startVertical + 620), 700, 72, document.GetRegularFont(true));
                AbsolutePositionText(document, string.Format(Resources.TXT_SIGNATURE_TECHNICIAN, tachographDocument.Technician), (startHorizontal + 300), (startVertical + 650), 550, 20);

                TryAddSignature(document, tachographDocument, (startHorizontal + 300));

                document.DrawLine((startHorizontal + 300), (startVertical + 715), (startHorizontal + 490), (startVertical + 715), TotalPageHeight);
                AbsolutePositionText(document, string.Format(Resources.TXT_DISTRIBUTOR_SEAL, RegistrationData.SealNumber), (startHorizontal + 300), (startVertical + 710), 550, 72, document.GetXSmallFont(false));


                Image image = Image.FromStream(DocumentHelper.GetResourceStreamFromSimplePath("../Images/webcal.jpg").Stream);
                document.AddImage(image.ToByteArray(), 90, 21, (startHorizontal + 5), (startVertical + 30));
            }
            else
            {
                AbsolutePositionText(document, Resources.TXT_TACHOGRAPH.ToUpper(), (startHorizontal + 5), (startVertical + 430), 550, 72, document.GetRegularFont(true));
                AbsolutePositionText(document, Resources.TXT_MAKE.ToUpper(), (startHorizontal + 140), (startVertical + 450), 550, 72, document.GetRegularFont(true));
                AbsolutePositionText(document, Resources.TXT_TYPE.ToUpper(), (startHorizontal + 140), (startVertical + 460), 550, 72, document.GetRegularFont(true));
                AbsolutePositionText(document, Resources.TXT_SERIAL_NUMBER.ToUpper(), (startHorizontal + 140), (startVertical + 470), 550, 72, document.GetRegularFont(true));

                AbsolutePositionText(document, tachographDocument.TachographMake.ToUpper(), (startHorizontal + 300), (startVertical + 450), 550, 72);
                AbsolutePositionText(document, tachographDocument.TachographModel.ToUpper(), (startHorizontal + 300), (startVertical + 460), 550, 72);
                AbsolutePositionText(document, tachographDocument.SerialNumber.ToUpper(), (startHorizontal + 300), (startVertical + 470), 550, 72);

                AbsolutePositionText(document, Resources.TXT_VEHICLE.ToUpper(), (startHorizontal + 5), (startVertical + 490), 550, 72, document.GetRegularFont(true));

                AbsolutePositionText(document, Resources.TXT_VRN.ToUpper(), (startHorizontal + 140), (startVertical + 510), 550, 72, document.GetRegularFont(true));
                AbsolutePositionText(document, Resources.TXT_VIN.ToUpper(), (startHorizontal + 140), (startVertical + 520), 550, 72, document.GetRegularFont(true));
                AbsolutePositionText(document, Resources.TXT_TYRE_SIZE.ToUpper(), (startHorizontal + 140), (startVertical + 530), 550, 72, document.GetRegularFont(true));
                AbsolutePositionText(document, Resources.TXT_W_FACTOR.ToUpper(), (startHorizontal + 140), (startVertical + 540), 550, 72, document.GetRegularFont(true));
                AbsolutePositionText(document, Resources.TXT_TYRE_L_FACTOR.ToUpper(), (startHorizontal + 140), (startVertical + 550), 550, 72, document.GetRegularFont(true));
                AbsolutePositionText(document, Resources.TXT_ODOMETER_READING.ToUpper(), (startHorizontal + 140), (startVertical + 560), 550, 72, document.GetRegularFont(true));
                AbsolutePositionText(document, Resources.TXT_SET_K_FACTOR.ToUpper(), (startHorizontal + 140), (startVertical + 570), 550, 72, document.GetRegularFont(true));

                AbsolutePositionText(document, string.Format("{0} {1}", tachographDocument.VehicleMake, tachographDocument.VehicleModel).ToUpper(), (startHorizontal + 140), (startVertical + 500), 550, 72, document.GetRegularFont(true));

                AbsolutePositionText(document, tachographDocument.RegistrationNumber.ToUpper(), (startHorizontal + 300), (startVertical + 510), 550, 72);
                AbsolutePositionText(document, tachographDocument.VIN.ToUpper(), (startHorizontal + 300), (startVertical + 520), 550, 72);
                AbsolutePositionText(document, tachographDocument.TyreSize.ToUpper(), (startHorizontal + 300), (startVertical + 530), 550, 72);
                AbsolutePositionText(document, string.Format(Resources.TXT_IMP_KM, tachographDocument.WFactor).ToUpper(), (startHorizontal + 300), (startVertical + 540), 550, 72);
                AbsolutePositionText(document, string.Format(Resources.TXT_MM, tachographDocument.LFactor).ToUpper(), (startHorizontal + 300), (startVertical + 550), 550, 72);
                AbsolutePositionText(document, tachographDocument.OdometerReading.ToUpper(), (startHorizontal + 300), (startVertical + 560), 550, 72);
                AbsolutePositionText(document, string.Format(Resources.TXT_IMP_KM, tachographDocument.KFactor).ToUpper(), (startHorizontal + 300), (startVertical + 570), 550, 72);

                AbsolutePositionText(document, Resources.TXT_AUTHORISED_TACHOGRAPH_CENTER, (startHorizontal + 30), (startVertical + 640), 550, 72, document.GetRegularFont(true));
                if (RegistrationData.CompanyName != null)
                {
                    AbsolutePositionText(document, RegistrationData.CompanyName, (startHorizontal + 30), (startVertical + 670), 550, 72, document.GetRegularFont(false));
                }
                if (WorkshopSettings.Address1 != null)
                {
                    AbsolutePositionText(document, WorkshopSettings.Address1, (startHorizontal + 30), (startVertical + 690), 550, 72, document.GetRegularFont(false));
                }
                if (WorkshopSettings.Town != null)
                {
                    AbsolutePositionText(document, WorkshopSettings.Town, (startHorizontal + 30), (startVertical + 710), 550, 72, document.GetRegularFont(false));
                }

                if (WorkshopSettings.PostCode != null)
                {
                    AbsolutePositionText(document, WorkshopSettings.PostCode, (startHorizontal + 30), (startVertical + 730), 550, 72, document.GetRegularFont(false));
                }

                AbsolutePositionText(document, Resources.TXT_AUTHORISED_TESTERS_SIGNATURE, (startHorizontal + 300), (startVertical + 640), 700, 72, document.GetRegularFont(true));
                AbsolutePositionText(document, string.Format(Resources.TXT_SIGNATURE_TECHNICIAN, tachographDocument.Technician), (startHorizontal + 300), (startVertical + 670), 550, 20);

                TryAddSignature(document, tachographDocument, (startHorizontal + 300));

                document.DrawLine((startHorizontal + 300), (startVertical + 735), (startHorizontal + 490), (startVertical + 735), TotalPageHeight);
                AbsolutePositionText(document, string.Format(Resources.TXT_DISTRIBUTOR_SEAL, RegistrationData.SealNumber), (startHorizontal + 300), (startVertical + 730), 550, 72, document.GetXSmallFont(false));

                Image image = Image.FromStream(DocumentHelper.GetResourceStreamFromSimplePath("../Images/webcal.jpg").Stream);
                document.AddImage(image.ToByteArray(), 90, 21, (startHorizontal + 5), (startVertical + 30));
            }
        }
    }
}