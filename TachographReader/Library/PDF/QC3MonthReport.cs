namespace TachographReader.Library.PDF
{
    using System;
    using Connect.Shared.Models;
    using Core;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using iTextSharp.text;
    using Properties;
    using Shared;
    using Shared.Helpers;

    public static class QC3MonthReport
    {
        public static void Create(PDFDocument document, QCReport3Month qcReport3Month)
        {
            AddImageFromResource(document, "skillray_small", 61, 770);
            AddImageFromResource(document, "webcal_print_logo", 200, 770);

            AbsolutePositionText(document, "Centre 3 monthly QC check", 400, 770, 560, 25, document.GetRegularFont(true, true), Element.ALIGN_LEFT);

            AbsolutePositionText(document, "Centre Name.", 61, 725, 480, 25, document.GetRegularFont(true), Element.ALIGN_LEFT);
            AbsolutePositionText(document, qcReport3Month.TachoCentreName, 150, 725, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, "Centre seal number.", 61, 700, 480, 25, document.GetRegularFont(true), Element.ALIGN_LEFT);
            AbsolutePositionText(document, qcReport3Month.CentreSealNumber, 150, 700, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, "The following document is to be completed and filed by the centre every 3 months to meet the requirements of Section 6 of the Approved Tachograph Centre Manual.  Any sections answered ‘no’ please state reason in comments box at end of document.", 61, 675, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, "Facilities ATCM Section 3", 61, 600, 480, 25, document.GetRegularFont(true, true), Element.ALIGN_LEFT);
            AddAnswerSection(document, Resources.TXT_QC_3_MONTH_SECTION_3_QUESTION_1, qcReport3Month.Section3Question1, 61, 575, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_3_MONTH_SECTION_3_QUESTION_2, qcReport3Month.Section3Question1, 61, 550, 480, 25);

            AbsolutePositionText(document, "Security ATCM section 4", 61, 525, 480, 25, document.GetRegularFont(true, true), Element.ALIGN_LEFT);
            AddAnswerSection(document, Resources.TXT_QC_3_MONTH_SECTION_4_QUESTION_1, qcReport3Month.Section4Question1, 61, 500, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_3_MONTH_SECTION_4_QUESTION_2, qcReport3Month.Section4Question2, 61, 475, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_3_MONTH_SECTION_4_QUESTION_3, qcReport3Month.Section4Question3, 61, 450, 480, 25);

            AbsolutePositionText(document, "Equipment ATCM section 5", 61, 425, 480, 25, document.GetRegularFont(true, true), Element.ALIGN_LEFT);
            AddAnswerSection(document, Resources.TXT_QC_3_MONTH_SECTION_5_QUESTION_1, qcReport3Month.Section5Question1, 61, 400, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_3_MONTH_SECTION_5_QUESTION_2, qcReport3Month.Section5Question1, 61, 375, 480, 25);

            AbsolutePositionText(document, "Technician training certificates ATCM section 7", 61, 350, 480, 25, document.GetRegularFont(true, true), Element.ALIGN_LEFT);
            AddAnswerSection(document, Resources.TXT_QC_3_MONTH_SECTION_7_QUESTION_1, qcReport3Month.Section7Question1, 61, 325, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_3_MONTH_SECTION_7_QUESTION_2, qcReport3Month.Section7Question1, 61, 300, 480, 25);
            
            AbsolutePositionText(document, "Calibration documentation ATCM section 9 & 10", 61, 275, 480, 25, document.GetRegularFont(true, true), Element.ALIGN_LEFT);
            AddAnswerSection(document, Resources.TXT_QC_3_MONTH_SECTION_9_10_QUESTION_1, qcReport3Month.CalibrationSection9And10Question1, 61, 250, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_3_MONTH_SECTION_9_10_QUESTION_2, qcReport3Month.CalibrationSection9And10Question2, 61, 225, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_3_MONTH_SECTION_9_10_QUESTION_3, qcReport3Month.CalibrationSection9And10Question3, 61, 200, 480, 25);

            AbsolutePositionText(document, "Tachograph centre data management ATCM section 10", 61, 175, 480, 25, document.GetRegularFont(true, true), Element.ALIGN_LEFT);
            AddAnswerSection(document, Resources.TXT_QC_3_MONTH_SECTION_10_QUESTION_1, qcReport3Month.DataManagementSection10Question1, 61, 150, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_3_MONTH_SECTION_10_QUESTION_2, qcReport3Month.DataManagementSection10Question2, 61, 125, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_3_MONTH_SECTION_10_QUESTION_3, qcReport3Month.DataManagementSection10Question3, 61, 100, 480, 25);

            document.AddPage();

            AbsolutePositionText(document, "Any answers, answered 'No' please state reasons in box below;", 61, 767, 500, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);
            document.DrawBox(61, 525, 464, 200);
            AbsolutePositionText(document, qcReport3Month.FurtherDetails, 71, 615, 480, 730, document.GetRegularFont(false), Element.ALIGN_LEFT);

            document.DrawBox(61, 450, 225, 35);
            AbsolutePositionText(document, $"{Resources.TXT_QC_REPORT_CHECK_TECHNICIAN} {qcReport3Month.Name}", 65, 480, 255, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            document.DrawBox(61, 400, 225, 35);
            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_SIGNATURE, 65, 430, 450, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);
            TryAddSignature(document, qcReport3Month.Name, 100, 390);

            document.DrawBox(61, 350, 225, 35);
            AbsolutePositionText(document, Resources.DATE + DateTime.Now.ToString(Constants.ShortYearDateFormat), 65, 380, 255, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);
        }

        private static void AddAnswerSection(PDFDocument document, string question, bool answer, float left, float top, float width, float height)
        {
            AbsolutePositionText(document, question, left, top, width, height, document.GetRegularFont(false), Element.ALIGN_LEFT);

            if (answer)
            {
                document.FillBox(400, top - 25, 40, 25, BaseColor.LIGHT_GRAY);
            }
            else
            {
                document.DrawBox(400, top - 25, 40, 25);
            }

            if (!answer)
            {
                document.FillBox(450, top - 25, 40, 25, BaseColor.LIGHT_GRAY);
            }
            else
            {
                document.DrawBox(450, top - 25, 40, 25);
            }

            AbsolutePositionText(document, Resources.TXT_YES, 412, top - 1, 450, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);
            AbsolutePositionText(document, Resources.TXT_NO, 464, top - 1, 500, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);
        }

        private static void AddImageFromResource(PDFDocument document, string resourceName, int x, int y)
        {
            var image = ImageHelper.LoadFromResources(resourceName);
            var bitmap = image.ToBitmap();
            var scaled = ImageHelper.Scale(bitmap, 0, 25);
            document.AddImage(scaled.ToByteArray(), scaled.Width, scaled.Height, x, y);
        }

        private static void AbsolutePositionText(PDFDocument document, string text, float left, float top, float width, float height, Font font, int alignment)
        {
            var absoluteColumn = document.GetNewColumn(left, top, width, height);
            document.AddParagraph(text, absoluteColumn, font, alignment);
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
    }
}
