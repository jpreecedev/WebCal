namespace TachographReader.Library.PDF
{
    using System;
    using Core;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using iTextSharp.text;
    using Properties;
    using Shared;
    using Shared.Helpers;
    using ViewModels;

    public static class QCCheckReport
    {
        public static void Create(PDFDocument document, QCReportViewModel qcReportDocument)
        {
            var settingsRepository = ContainerBootstrapper.Resolve<ISettingsRepository<WorkshopSettings>>();
            var settings = settingsRepository.GetWorkshopSettings();

            AddImageFromResource(document, "skillray_small", 61, 770);
            AddImageFromResource(document, "traintec_small", 350, 770);

            AbsolutePositionText(document, Resources.TXT_QC_CHECK_REPORT_TACHO_CENTRE_DETAILS, 61, 747, 500, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);

            document.DrawBox(105, 700, 380, 25);
            AbsolutePositionText(document, settings.WorkshopName, 110, 725, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            document.DrawBox(105, 675, 380, 25);
            AbsolutePositionText(document, settings.Address1, 110, 700, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            document.DrawBox(105, 650, 380, 25);
            AbsolutePositionText(document, settings.Address2, 110, 675, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            document.DrawBox(105, 625, 380, 25);
            AbsolutePositionText(document, settings.Town, 110, 650, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            document.DrawBox(105, 600, 380, 25);
            AbsolutePositionText(document, settings.PostCode, 110, 625, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_TACHO_MANAGER_NAME, 61, 580, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 555, 250, 25);
            AbsolutePositionText(document, qcReportDocument.TachoManagerName, 305, 580, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_QC_MANAGER_NAME, 61, 540, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 515, 250, 25);
            AbsolutePositionText(document, qcReportDocument.QCManagerName, 305, 540, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_TECHNICIAN_NAME, 61, 500, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 475, 250, 25);
            AbsolutePositionText(document, qcReportDocument.TechnicianName, 305, 500, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_DATE_OF_AUDIT, 61, 460, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 435, 250, 25);
            AbsolutePositionText(document, qcReportDocument.DateOfAudit.ToString(Constants.ShortYearDateFormat), 305, 460, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_TYPE_OF_CHECK, 61, 410, 480, 25, document.GetLargeFont(true), Element.ALIGN_LEFT);

            AbsolutePositionText(document, QCReportViewModel.InitialInstallationAnalogue, 61, 380, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 355, 40, 25);
            AbsolutePositionText(document, qcReportDocument.IsDocumentType(QCReportViewModel.InitialInstallationAnalogue) ? Resources.TXT_X : string.Empty, 315, 379, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, QCReportViewModel.SixYearCalibrationAnalogue, 61, 350, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 325, 40, 25);
            AbsolutePositionText(document, qcReportDocument.IsDocumentType(QCReportViewModel.SixYearCalibrationAnalogue) ? Resources.TXT_X : string.Empty, 315, 349, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, QCReportViewModel.TwoYearInspectionAnalogue, 61, 320, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 295, 40, 25);
            AbsolutePositionText(document, qcReportDocument.IsDocumentType(QCReportViewModel.TwoYearInspectionAnalogue) ? Resources.TXT_X : string.Empty, 315, 319, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, QCReportViewModel.InitialInstallationDigital, 61, 275, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 250, 40, 25);
            AbsolutePositionText(document, qcReportDocument.IsDocumentType(QCReportViewModel.InitialInstallationDigital) ? Resources.TXT_X : string.Empty, 315, 274, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, QCReportViewModel.TwoYearCalibrationDigital, 61, 245, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 220, 40, 25);
            AbsolutePositionText(document, qcReportDocument.IsDocumentType(QCReportViewModel.TwoYearCalibrationDigital) ? Resources.TXT_X : string.Empty, 315, 244, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_TACHOGRAPH_DETAILS, 61, 195, 480, 25, document.GetLargeFont(true), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_TACHO_MAKE, 61, 168, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 145, 250, 25);
            AbsolutePositionText(document, qcReportDocument.TachographMake, 305, 169, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_TACHO_TYPE, 61, 145, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 120, 250, 25);
            AbsolutePositionText(document, qcReportDocument.TachographModel, 305, 145, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_TACHO_SERIAL, 61, 118, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 95, 250, 25);
            AbsolutePositionText(document, qcReportDocument.TachographSerialNumber, 305, 119, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            document.AddPage();

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_VEHICLE_DETAILS, 61, 767, 500, 25, document.GetLargeFont(true), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_TACHO_MAKE, 61, 740, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 715, 250, 25);
            AbsolutePositionText(document, qcReportDocument.VehicleMake, 305, 740, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_VEHICLE_TYPE, 61, 715, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 690, 250, 25);
            AbsolutePositionText(document, qcReportDocument.VehicleType, 305, 715, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_VEHICLE_REGISTRATION, 61, 690, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 665, 250, 25);
            AbsolutePositionText(document, qcReportDocument.VehicleRegistrationNumber, 305, 690, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_VIN, 61, 665, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 640, 250, 25);
            AbsolutePositionText(document, qcReportDocument.VehicleIdentificationNumber, 305, 665, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_CALIBRATION_INSPECTION, 61, 625, 500, 25, document.GetLargeFont(true), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_3_BASIC_CHECKS_COMPLETED, 61, 605, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 580, 40, 25);
            AbsolutePositionText(document, qcReportDocument.ThreeBasicChecksCompleted ? Resources.TXT_X : string.Empty, 315, 605, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_ESTABLISH_W_FACTOR, 61, 570, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 545, 80, 25);
            AbsolutePositionText(document, qcReportDocument.WFactor == null ? string.Empty : qcReportDocument.WFactor.ToString(), 310, 570, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);
            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_IMP_KM, 390, 570, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_ESTABLISH_L, 61, 545, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 520, 80, 25);
            AbsolutePositionText(document, qcReportDocument.LFactor == null ? string.Empty : qcReportDocument.LFactor.ToString(), 310, 545, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);
            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_MM, 390, 545, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_K_FACTOR, 61, 520, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 495, 80, 25);
            AbsolutePositionText(document, qcReportDocument.KFactor == null ? string.Empty : qcReportDocument.KFactor.ToString(), 310, 520, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);
            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_IMP_KM, 390, 520, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_40_KM_TEST, 61, 495, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 470, 80, 25);
            AbsolutePositionText(document, qcReportDocument.FortyKmTest ?? string.Empty, 310, 495, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);
            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_KMH, 390, 495, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_60_KM_TEST, 61, 470, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            document.DrawBox(300, 445, 80, 25);
            AbsolutePositionText(document, qcReportDocument.SixtyKmTest ?? string.Empty, 310, 470, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);
            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_KMH, 390, 470, 480, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);

            AddAnswerSection(document, Resources.TXT_QC_REPORT_CHECK_CLOCK_TEST_COMPLETED, true, qcReportDocument.ClockTestCompleted, 61, 425, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_REPORT_CHECK_BENCH_TEST_CARRIED_OUT, true, qcReportDocument.BenchTestCarriedOutAnalogue, 61, 390, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_REPORT_CHECK_FUNCTIONAL_BENCH_TEST, true, qcReportDocument.FunctionalBenchTestDigital, 61, 355, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_REPORT_CHECK_DISTANCE_CHECK_CARRIED_OUT, true, qcReportDocument.DistanceCheckCarriedOut, 61, 320, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_REPORT_CHECK_TEST_CHARTS_COMPLETED, true, qcReportDocument.TestChartsCompleted, 61, 285, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_REPORT_CHECK_SPEED_FOR_SPEED_CHECK, true, qcReportDocument.SpeedForSpeedCheckCompleted, 61, 250, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_REPORT_CHECK_SYSTEM_SEALED_IN_ACCORDANCE, false, qcReportDocument.SystemSealedInAccordance, 61, 215, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_REPORT_CHECK_CALIBRATION_CERTIFICATE_COMPLETED, false, qcReportDocument.CalibrationCertificateCompleted, 61, 180, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_REPORT_CHECK_REFERENCE_CABLE_CHECK, true, qcReportDocument.ReferenceCableCheckCompleted, 61, 145, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_REPORT_CHECK_TECHNICAL_DATA_PRINTOUTS, true, qcReportDocument.TechnicalDataPrintoutsCreated, 61, 110, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_REPORT_CHECK_EVENTS_FAULTS_READ, true, qcReportDocument.EventsFaultsReadCleared, 61, 75, 480, 25);

            document.AddPage();

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_COMMENTS, 61, 767, 500, 25, document.GetLargeFont(true), Element.ALIGN_LEFT);

            document.DrawBox(105, 525, 380, 200);
            AbsolutePositionText(document, qcReportDocument.Comments, 110, 515, 480, 730, document.GetRegularFont(false), Element.ALIGN_LEFT);

            document.DrawBox(200, 465, 50, 40);
            document.DrawBox(350, 465, 50, 40);

            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_PASS, 155, 500, 255, 35, document.GetLargeFont(true), Element.ALIGN_LEFT);
            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_FAIL, 315, 500, 415, 35, document.GetLargeFont(true), Element.ALIGN_LEFT);

            if (qcReportDocument.Passed)
            {
                AbsolutePositionText(document, Resources.TXT_X, 219, 500, 255, 35, document.GetXLargeFont(true), Element.ALIGN_LEFT);
            }
            if (!qcReportDocument.Passed)
            {
                AbsolutePositionText(document, Resources.TXT_X, 369, 500, 405, 35, document.GetXLargeFont(true), Element.ALIGN_LEFT);
            }
            
            document.DrawBox(61, 350, 225, 35);
            AbsolutePositionText(document, $"{Resources.TXT_QC_REPORT_CHECK_TECHNICIAN} {qcReportDocument.TechnicianName}", 65, 380, 255, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);
            
            document.DrawBox(310, 350, 225, 35);
            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_SIGNATURE, 315, 380, 450, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);
            TryAddSignature(document, qcReportDocument.TechnicianName, 345, 340);

            document.DrawBox(61, 300, 225, 35);
            AbsolutePositionText(document, $"{Resources.TXT_QC_REPORT_CHECK_QC} {qcReportDocument.QCManagerName}", 65, 330, 255, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            document.DrawBox(310, 300, 225, 35);
            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_SIGNATURE, 315, 330, 450, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);
            TryAddSignature(document, qcReportDocument.QCManagerName, 345, 290);

            document.DrawBox(61, 250, 225, 35);
            AbsolutePositionText(document, Resources.DATE + DateTime.Now.ToString(Constants.ShortYearDateFormat), 65, 280, 255, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);
        }

        private static void AddAnswerSection(PDFDocument document, string question, bool hasNotApplicable, bool? answer, float left, float top, float width, float height)
        {
            AbsolutePositionText(document, question, left, top, width, height, document.GetLargeFont(false), Element.ALIGN_LEFT);

            if (answer.HasValue && answer.Value)
            {
                document.FillBox(300, top - 25, 80, 25, BaseColor.LIGHT_GRAY);
            }
            else
            {
                document.DrawBox(300, top - 25, 80, 25);
            }

            if (answer.HasValue && !answer.Value)
            {
                document.FillBox(380, top - 25, 80, 25, BaseColor.LIGHT_GRAY);
            }
            else
            {
                document.DrawBox(380, top - 25, 80, 25);
            }
            
            AbsolutePositionText(document, Resources.TXT_YES, 500, top - 1, 305, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            AbsolutePositionText(document, Resources.TXT_NO, 600, top - 1, 385, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);

            if (hasNotApplicable)
            {
                if (!answer.HasValue)
                {
                    document.FillBox(460, top - 25, 80, 25, BaseColor.LIGHT_GRAY);
                }
                else
                {
                    document.DrawBox(460, top - 25, 80, 25);
                }

                AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_NA, 700, top - 1, 465, 25, document.GetLargeFont(false), Element.ALIGN_LEFT);
            }
        }

        private static void TryAddSignature(PDFDocument document, string technicianName, int x, int y = 88)
        {
            System.Drawing.Image signatureImage = null;

            var userRepository = ContainerBootstrapper.Resolve<IRepository<User>>();
            var user = UserManagement.GetUser(userRepository, UserManagement.LoggedInUserName);
            if (user != null && user.Image != null)
            {
                signatureImage = user.Image;
            }

            IRepository<Technician> technicianRepository = ContainerBootstrapper.Resolve<IRepository<Technician>>();
            var technicianUser = technicianRepository.FirstOrDefault(c => string.Equals(c.Name, technicianName));
            if (technicianUser != null && technicianUser.Image != null)
            {
                signatureImage = technicianUser.Image;
            }

            if (signatureImage != null)
            {
                var image = ImageHelper.Scale(signatureImage, 500, 50);
                document.AddImage(image.ToByteArray(), image.Width, image.Height, x, y);
            }
        }

        private static void AddImageFromResource(PDFDocument document, string resourceName, int x, int y)
        {
            var image = ImageHelper.LoadFromResources(resourceName);
            var bitmap = image.ToBitmap();
            var scaled = ImageHelper.Scale(bitmap, 0, 35);
            document.AddImage(scaled.ToByteArray(), scaled.Width, scaled.Height, x, y);
        }

        private static void AbsolutePositionText(PDFDocument document, string text, float left, float top, float width, float height, Font font, int alignment)
        {
            var absoluteColumn = document.GetNewColumn(left, top, width, height);
            document.AddParagraph(text, absoluteColumn, font, alignment);
        }
    }
}
