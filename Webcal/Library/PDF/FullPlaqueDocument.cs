namespace Webcal.Library.PDF
{
    using System.Drawing;
    using DataModel;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using Properties;

    public class FullPlaqueDocument : BasePlaqueDocument
    {
        protected override string GetTitle()
        {
            return Resources.TXT_TACHOGRAPH_AND_M1N1_INSPECTION_PLATE;
        }

        protected override void CreateLargeLabel(PDFDocument document, TachographDocument tachographDocument, int startHorizontal, int startVertical)
        {
            AbsolutePositionText(document, Resources.TXT_CERTIFICATION_INSPECTION_SEALING_TACHOGRAPH_M1N1, (startHorizontal + 32), (startVertical + 0), 580, 100, document.GetXLargeFont(false), Element.ALIGN_CENTER);

            //Outlining
            DrawLargeLabelRectangle(document, startHorizontal, startVertical);

            document.DrawLine((startHorizontal + 50), (startVertical + 106), (startHorizontal + 545), (startVertical + 106), TotalPageHeight);
            document.DrawLine((startHorizontal + 50), (startVertical + 192), (startHorizontal + 545), (startVertical + 192), TotalPageHeight);
            document.DrawLine((startHorizontal + 50), (startVertical + 247), (startHorizontal + 545), (startVertical + 247), TotalPageHeight);
            document.DrawLine((startHorizontal + 150), (startVertical + 344), (startHorizontal + 298), (startVertical + 344), TotalPageHeight);
            document.DrawLine((startHorizontal + 347), (startVertical + 344), (startHorizontal + 494), (startVertical + 344), TotalPageHeight);

            AbsolutePositionText(document, Resources.TXT_TACHOGRAPH, (startHorizontal + 50), (startVertical + 52), 150, 20);
            AbsolutePositionText(document, Resources.TXT_VEHICLE, (startHorizontal + 50), (startVertical + 137), 150, 20);
            AbsolutePositionText(document, Resources.TXT_M1N1_ADAPTER, (startHorizontal + 50), (startVertical + 208), 150, 20);
            AbsolutePositionText(document, Resources.TXT_TECHNICIAN, (startHorizontal + 50), (startVertical + 270), 150, 20);
            AbsolutePositionText(document, Resources.TXT_DATE, (startHorizontal + 150), (startVertical + 340), 200, 20);

            TryAddSignature(document, (startHorizontal + 330));
            AbsolutePositionText(document, string.Format(Resources.TXT_SIGNATURE_TECHNICIAN, tachographDocument.Technician), (startHorizontal + 347), (startVertical + 340), 550, 20);

            //Tachograph
            AbsolutePositionText(document, Resources.TXT_MAKE, (startHorizontal + 150), (startVertical + 20), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, Resources.TXT_TYPE, (startHorizontal + 347), (startVertical + 20), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, tachographDocument.TachographMake, (startHorizontal + 150), (startVertical + 32), 550, 72, document.GetLargerFont(false));
            AbsolutePositionText(document, tachographDocument.TachographModel, (startHorizontal + 347), (startVertical + 32), 550, 72, document.GetLargerFont(false));

            AbsolutePositionText(document, Resources.TXT_ODOMETER_READING, (startHorizontal + 150), (startVertical + 46), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, Resources.TXT_SERIAL_NUMBER, (startHorizontal + 347), (startVertical + 46), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, tachographDocument.OdometerReading, (startHorizontal + 150), (startVertical + 58), 550, 72, document.GetLargerFont(false));
            AbsolutePositionText(document, tachographDocument.SerialNumber, (startHorizontal + 347), (startVertical + 58), 550, 72, document.GetLargerFont(false));

            AbsolutePositionText(document, Resources.TXT_SET_K_FACTOR, (startHorizontal + 150), (startVertical + 72), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, tachographDocument.KFactor, (startHorizontal + 150), (startVertical + 84), 550, 72, document.GetLargerFont(false));

            //Vehicle

            AbsolutePositionText(document, Resources.TXT_MAKE_AND_MODEL, (startHorizontal + 150), (startVertical + 104), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, Resources.TXT_VRN, (startHorizontal + 347), (startVertical + 104), 550, 72, document.GetRegularFont(false));

            AbsolutePositionText(document, string.Format("{0} {1}", tachographDocument.VehicleMake, tachographDocument.VehicleModel), (startHorizontal + 150), (startVertical + 116), 550, 72, document.GetLargerFont(false));
            AbsolutePositionText(document, tachographDocument.RegistrationNumber, (startHorizontal + 347), (startVertical + 116), 550, 72, document.GetLargerFont(false));

            AbsolutePositionText(document, Resources.TXT_VIN, (startHorizontal + 150), (startVertical + 130), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, Resources.TXT_TYRE_SIZE, (startHorizontal + 347), (startVertical + 130), 550, 72, document.GetRegularFont(false));

            AbsolutePositionText(document, tachographDocument.VIN, (startHorizontal + 150), (startVertical + 142), 550, 72, document.GetLargerFont(false));
            AbsolutePositionText(document, tachographDocument.TyreSize, (startHorizontal + 347), (startVertical + 142), 550, 72, document.GetLargerFont(false));

            AbsolutePositionText(document, Resources.TXT_W_FACTOR, (startHorizontal + 150), (startVertical + 156), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, Resources.TXT_TYRE_L_FACTOR, (startHorizontal + 347), (startVertical + 156), 550, 72, document.GetRegularFont(false));

            AbsolutePositionText(document, string.Format(Resources.TXT_IMP_KM, tachographDocument.WFactor), (startHorizontal + 150), (startVertical + 168), 550, 72, document.GetLargerFont(false));
            AbsolutePositionText(document, string.Format(Resources.TXT_MM, tachographDocument.LFactor), (startHorizontal + 347), (startVertical + 168), 550, 72, document.GetLargerFont(false));

            //M1N1 Adapter

            AbsolutePositionText(document, Resources.TXT_M1N1_SERIAL_NUMBER, (startHorizontal + 150), (startVertical + 188), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, Resources.TXT_COLOR, (startHorizontal + 347), (startVertical + 188), 550, 72, document.GetRegularFont(false));

            AbsolutePositionText(document, tachographDocument.TachographAdapterSerialNumber, (startHorizontal + 150), (startVertical + 200), 550, 72, document.GetLargerFont(false));
            AbsolutePositionText(document, tachographDocument.TachographCableColor, (startHorizontal + 347), (startVertical + 200), 550, 72, document.GetLargerFont(false));

            AbsolutePositionText(document, Resources.TXT_LOCATION, (startHorizontal + 150), (startVertical + 214), 550, 72, document.GetRegularFont(false));

            AbsolutePositionText(document, tachographDocument.TachographAdapterLocation, (startHorizontal + 150), (startVertical + 226), 550, 72, document.GetLargerFont(false));

            //Technician

            AbsolutePositionText(document, RegistrationData.CompanyName, (startHorizontal + 150), (startVertical + 246), 550, 72, document.GetLargerFont(false));
            AbsolutePositionText(document, WorkshopSettings.Address1, (startHorizontal + 150), (startVertical + 270), 550, 72, document.GetLargerFont(false));
            AbsolutePositionText(document, string.Format("{0} {1}", WorkshopSettings.Town, WorkshopSettings.PostCode), (startHorizontal + 150), (startVertical + 284), 550, 72, document.GetLargerFont(false));

            AbsolutePositionText(document, string.Format(Resources.TXT_DISTRIBUTOR_SEAL, RegistrationData.SealNumber), (startHorizontal + 347), (startVertical + 246), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, GetCalibrationTime(tachographDocument.CalibrationTime), (startHorizontal + 150), (startVertical + 324), 250, 40, document.GetLargerFont(false));
        }

        protected override void DrawLargeLabelRectangle(PDFDocument document, int startHorizontal, int startVertical)
        {
            int height = 337;

            document.ContentByte.SetColorStroke(new BaseColor(Color.Black));
            document.ContentByte.Rectangle((startHorizontal + 20), (TotalPageHeight - (startVertical + 25 + height)), 555, height);
            document.ContentByte.Stroke();
        }

        protected override void CreateKFactorLabel(PDFDocument document, TachographDocument tachographDocument)
        {
            ColumnText kValueColumn = document.GetNewColumn(425, document.Height - 356, 480, 100);
            document.AddParagraph(tachographDocument.KFactor, kValueColumn);
        }

        protected override void CreateMediumLabel(PDFDocument document, TachographDocument tachographDocument)
        {
            var table = new PdfPTable(4);
            table.TotalWidth = 266;
            table.SetWidths(new float[] {108, 54, 54, 108});

            GetWorkshopImage(document, table);

            document.AddSpannedCell(table, Resources.TXT_TACHOGRAPH_AND_M1N1_INSPECTION_PLATE, 2, document.GetSmallerFont(), 29);

            document.AddSpannedCell(table, string.Format(Resources.TXT_W_FACTOR_MEDIUM_LABEL, tachographDocument.WFactor), 1, document.GetXSmallFont(false), 16);
            document.AddSpannedCell(table, string.Format(Resources.TXT_K_FACTOR_MEDIUM_LABEL, tachographDocument.KFactor), 2, document.GetXSmallFont(false), 16);
            document.AddSpannedCell(table, string.Format(Resources.TXT_L_FACTOR_MEDIUM_LABEL, tachographDocument.LFactor), 1, document.GetXSmallFont(false), 16);

            document.AddSpannedCell(table, string.Format(Resources.TXT_VIN_MEDIUM_LABEL, tachographDocument.VIN), 2, document.GetSmallFont(false), 16, 68);
            document.AddSpannedCell(table, string.Format(Resources.TXT_TYRE_SIZE_MEDIUM_LABEL, tachographDocument.TyreSize), 2, document.GetSmallFont(false), 17, Element.ALIGN_LEFT);

            document.AddSpannedCell(table, string.Format(Resources.TXT_SERIAL_NUMBER_MEDIUM_LABEL, tachographDocument.SerialNumber), 2, document.GetSmallFont(false), 17, Element.ALIGN_LEFT);
            document.AddSpannedCell(table, string.Format(Resources.TXT_M1N1_SERIAL_NUMBER_MEDIUM_LABEL, tachographDocument.TachographAdapterSerialNumber), 2, document.GetSmallFont(false), 18, Element.ALIGN_LEFT);
            document.AddSpannedCell(table, string.Format(Resources.TXT_LOCATION_MEDIUM_LABEL, tachographDocument.TachographAdapterLocation), 2, document.GetSmallFont(false), 16, Element.ALIGN_LEFT);
            document.AddSpannedCell(table, string.Format(Resources.TXT_COLOUR_MEDIUM_LABEL, tachographDocument.TachographCableColor), 2, document.GetSmallFont(false), 16, Element.ALIGN_LEFT);

            GetCompanyDetails(document, table, 30, 3, document.GetSmallerFont());

            document.AddSpannedCell(table, "date\n" + GetCalibrationTime(tachographDocument.CalibrationTime), 1, document.GetRegularFont(false));
            document.AddSpannedCell(table, GetLicenseNumberParagraph(document, true), 4, 12, Element.ALIGN_LEFT);

            table.WriteSelectedRows(0, -1, document.Document.LeftMargin + 193, document.Height - 103, document.ContentByte);
        }

        protected override void CreateSmallLabel(PDFDocument document, TachographDocument tachographDocument)
        {
            var table = new PdfPTable(4);
            table.TotalWidth = 136;
            table.SetWidths(new float[] {58, 10, 10, 58});

            GetSmallImage(document, table);

            document.AddSpannedCell(table, Resources.TXT_TACHOGRAPH_AND_M1N1_INSPECTION_PLATE, 4, document.GetSmallerFont(), 29);

            document.AddSpannedCell(table, string.Format(Resources.TXT_W_FACTOR_IMP_KM, tachographDocument.WFactor), 2, document.GetXSmallFont(false), 15);
            document.AddSpannedCell(table, string.Format(Resources.TXT_K_FACTOR_IMP_KM, tachographDocument.KFactor), 2, document.GetXSmallFont(false), 15);

            document.AddSpannedCell(table, string.Format(Resources.TXT_L_FACTOR_MM, tachographDocument.LFactor), 1, document.GetXSmallFont(false), 15, 58);
            document.AddSpannedCell(table, string.Format(Resources.TXT_VIN_SMALL_LABEL, tachographDocument.VIN), 3, document.GetXXSmallFont(), 16, 68);

            document.AddSpannedCell(table, string.Format(Resources.TXT_TYRE_SIZE_SMALL_LABEL, tachographDocument.TyreSize), 4, document.GetSmallFont(false), 17, Element.ALIGN_LEFT);
            document.AddSpannedCell(table, string.Format(Resources.TXT_SERIAL_NUMBER_SMALL_LABEL, tachographDocument.SerialNumber), 4, document.GetSmallFont(false), 17, Element.ALIGN_LEFT);

            document.AddSpannedCell(table, string.Format(Resources.TXT_M1N1_SERIAL_SMALL_LABEL, tachographDocument.TachographAdapterSerialNumber), 4, document.GetSmallFont(false), 18, Element.ALIGN_LEFT);
            document.AddSpannedCell(table, string.Format(Resources.TXT_LOCATION_SMALL_LABEL, tachographDocument.TachographAdapterLocation), 4, document.GetSmallFont(false), 16, Element.ALIGN_LEFT);
            document.AddSpannedCell(table, string.Format(Resources.TXT_COLOR_SMALL_LABEL, tachographDocument.TachographCableColor), 4, document.GetSmallFont(false), 16, Element.ALIGN_LEFT);

            GetCompanyDetails(document, table, 69, 3, document.GetSmallerFont());

            document.AddSpannedCell(table, "\ndate\n\n\n" + GetCalibrationTime(tachographDocument.CalibrationTime), 1, document.GetRegularFont(false));
            document.AddSpannedCell(table, GetLicenseNumberParagraph(document, false), 4, 16, Element.ALIGN_LEFT);

            table.WriteSelectedRows(0, -1, document.Document.LeftMargin + 2, document.Height - 103, document.ContentByte);
        }
    }
}