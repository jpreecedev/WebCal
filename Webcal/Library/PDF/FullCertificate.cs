namespace Webcal.Library.PDF
{
    using System;
    using DataModel;
    using iTextSharp.text;
    using Properties;

    public class FullCertificate : BasePlaqueDocument
    {
        protected override string GetTitle()
        {
            return Resources.TXT_TACHOGRAPH_AND_M1N1_INSPECTION_PLATE;
        }

        protected override void CreateLargeCertificate(PDFDocument document, TachographDocument tachographDocument)
        {
            const int startHorizontal = 0;
            const int startVertical = 0;

            document.DrawLine((startHorizontal), (startVertical), (startHorizontal + 545), (startVertical), TotalPageHeight);

            document.DrawLine((startHorizontal), (startVertical), (startHorizontal), (startVertical + 790), TotalPageHeight);

            document.DrawLine((startHorizontal + 545), (startVertical), (startHorizontal + 545), (startVertical + 790), TotalPageHeight);

            document.DrawLine((startHorizontal), (startVertical + 790), (startHorizontal + 545), (startVertical + 790), TotalPageHeight);

            document.DrawLine((startHorizontal), (startVertical + 290), (startHorizontal + 545), (startVertical + 290), TotalPageHeight);
            document.DrawLine((startHorizontal), (startVertical + 400), (startHorizontal + 545), (startVertical + 400), TotalPageHeight);
            document.DrawLine((startHorizontal), (startVertical + 600), (startHorizontal + 545), (startVertical + 600), TotalPageHeight);
            
            if (WorkshopSettings.Image != null)
            {
                const float profileImageMaxHeight = 150;
                const float profileImageMaxWidth = 200;

                float widthScale = profileImageMaxWidth / WorkshopSettings.Image.Width;
                float heightScale = profileImageMaxHeight / WorkshopSettings.Image.Height;
                float scale = Math.Min(widthScale, heightScale);
                float newWidth = WorkshopSettings.Image.Width * scale;
                float newHeight = WorkshopSettings.Image.Height * scale;

                document.AddImage(WorkshopSettings.RawImage, newWidth, newHeight, (startHorizontal + 5), startVertical + 5);
            }

            AbsolutePositionText(document, Resources.TXT_TACHOGRAPH_CALIBRATION_CERTIFICATE, (startHorizontal + 32), (startVertical + 0), 580, 100, document.GetXLargeFont(false), Element.ALIGN_CENTER);

            AbsolutePositionText(document, Resources.TXT_DATE, (startHorizontal + 355), (startVertical + 200), 200, 40);
            AbsolutePositionText(document, GetCalibrationTime(tachographDocument.CalibrationTime), (startHorizontal + 400), (startVertical + 200), 200, 40);

            DateTime? calibrationDate = tachographDocument.CalibrationTime;

            if (calibrationDate == null)
            {
                calibrationDate = DateTime.Today;
            }

            string expiryDate = "EXP : " + (GetCalibrationTime(calibrationDate.Value.AddYears(2).AddDays(-1)));
            AbsolutePositionText(document, expiryDate, (startHorizontal + 355), (startVertical + 200), 580, 100);

            AbsolutePositionText(document, Resources.TXT_ANALOGUE_INSPECTIONS, (startHorizontal + 320), (startVertical + 5), 200, 40);

            AbsolutePositionText(document, Resources.TXT_DIGITAL_INSPECTIONS, (startHorizontal + 320), (startVertical + 240), 200, 40);

            AbsolutePositionText(document, Resources.TXT_MAKE, (startHorizontal + 150), (startVertical + 20), 550, 72, document.GetRegularFont(true));

            AbsolutePositionText(document, Resources.TXT_TYPE, (startHorizontal + 347), (startVertical + 20), 550, 72, document.GetRegularFont(true));

            AbsolutePositionText(document, Resources.TXT_SERIAL_NUMBER, (startHorizontal + 347), (startVertical + 46), 550, 72, document.GetRegularFont(true));

            AbsolutePositionText(document, tachographDocument.TachographMake, (startHorizontal + 150), (startVertical + 32), 550, 72);

            AbsolutePositionText(document, tachographDocument.TachographModel, (startHorizontal + 347), (startVertical + 32), 550, 72);

            AbsolutePositionText(document, tachographDocument.SerialNumber, (startHorizontal + 347), (startVertical + 58), 550, 72);

            AbsolutePositionText(document, Resources.TXT_VEHICLE, (startHorizontal + 5), (startVertical + 104), 550, 72, document.GetRegularFont(false));

            AbsolutePositionText(document, Resources.TXT_VRN, (startHorizontal + 170), (startVertical + 510), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, Resources.TXT_VIN, (startHorizontal + 170), (startVertical + 520), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, Resources.TXT_TYRE_SIZE, (startHorizontal + 170), (startVertical + 530), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, Resources.TXT_W_FACTOR, (startHorizontal + 170), (startVertical + 540), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, Resources.TXT_TYRE_L_FACTOR, (startHorizontal + 170), (startVertical + 550), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, Resources.TXT_ODOMETER_READING, (startHorizontal + 170), (startVertical + 560), 550, 72, document.GetRegularFont(false));
            AbsolutePositionText(document, Resources.TXT_SET_K_FACTOR, (startHorizontal + 170), (startVertical + 72), 570, 72, document.GetRegularFont(false));

            AbsolutePositionText(document, string.Format("{0} {1}", tachographDocument.VehicleMake, tachographDocument.VehicleModel), (startHorizontal + 170), (startVertical + 500), 550, 72, document.GetLargerFont(false));

            AbsolutePositionText(document, tachographDocument.RegistrationNumber, (startHorizontal + 240), (startVertical + 510), 550, 72);
            AbsolutePositionText(document, tachographDocument.VIN, (startHorizontal + 240), (startVertical + 520), 550, 72);
            AbsolutePositionText(document, tachographDocument.TyreSize, (startHorizontal + 240), (startVertical + 530), 550, 72);
            AbsolutePositionText(document, string.Format(Resources.TXT_IMP_KM, tachographDocument.WFactor), (startHorizontal + 240), (startVertical + 540), 550, 72);
            AbsolutePositionText(document, string.Format(Resources.TXT_MM, tachographDocument.LFactor), (startHorizontal + 240), (startVertical + 550), 550, 72);
            AbsolutePositionText(document, tachographDocument.OdometerReading, (startHorizontal + 240), (startVertical + 560), 550, 72);
            AbsolutePositionText(document, tachographDocument.KFactor, (startHorizontal + 240), (startVertical + 570), 550, 72);

            AbsolutePositionText(document, Resources.TXT_AUTHORISED_TACHOGRAPH_CENTER, (startHorizontal + 30), (startVertical + 650), 550, 72, document.GetLargerFont(false));

            AbsolutePositionText(document, RegistrationData.CompanyName, (startHorizontal + 30), (startVertical + 670), 550, 72, document.GetLargerFont(false));

            AbsolutePositionText(document, WorkshopSettings.Address1, (startHorizontal + 30), (startVertical + 700), 550, 72, document.GetLargerFont(false));

            AbsolutePositionText(document, string.Format("{0} {1}", WorkshopSettings.Town, WorkshopSettings.PostCode), (startHorizontal + 30), (startVertical + 730), 550, 72, document.GetLargerFont(false));

            AbsolutePositionText(document, string.Format(Resources.TXT_DISTRIBUTOR_SEAL, RegistrationData.SealNumber), (startHorizontal + 30), (startVertical + 760), 550, 72, document.GetRegularFont(false));

            AbsolutePositionText(document, Resources.TXT_AUTHORISED_TESTERS_SIGNATURE, (startHorizontal + 350), (startVertical + 246), 550, 72, document.GetLargerFont(false));

            TryAddSignature(document, (startHorizontal + 350));
            AbsolutePositionText(document, string.Format(Resources.TXT_SIGNATURE_TECHNICIAN, tachographDocument.Technician), (startHorizontal + 350), (startVertical + 340), 550, 20);

            string imageFilePath = (DocumentHelper.GetResourceStreamFromSimplePath("Images/PDF/skillray-small.png")).ToString();
            iTextSharp.text.Image logoImage = iTextSharp.text.Image.GetInstance(imageFilePath);
            logoImage.SetAbsolutePosition(50, 790);
        }
    }
}