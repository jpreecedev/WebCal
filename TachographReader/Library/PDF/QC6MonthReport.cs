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

    public static class QC6MonthReport
    {
        public static void Create(PDFDocument document, QCReport6Month qcReport6Month)
        {
            AddImageFromResource(document, "skillray_small", 61, 770);
            AddImageFromResource(document, "webcal_print_logo", 200, 770);

            AbsolutePositionText(document, Resources.TXT_QC_6_MONTH_CENTRE_QC_CHECK, 400, 770, 560, 25, document.GetRegularFont(true, true), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_6_MONTH_CENTRE_NAME, 61, 725, 480, 25, document.GetRegularFont(true), Element.ALIGN_LEFT);
            AbsolutePositionText(document, qcReport6Month.CentreName, 150, 725, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_6_MONTH_CENTRE_SEAL_NUMBER, 61, 700, 480, 25, document.GetRegularFont(true), Element.ALIGN_LEFT);
            AbsolutePositionText(document, qcReport6Month.CentreSealNumber, 150, 700, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_6_MONTH_DECLARATION, 61, 675, 480, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_QC_6_MONTH_SECTION_3, 61, 600, 480, 25, document.GetRegularFont(true, true), Element.ALIGN_LEFT);
            AddAnswerSection(document, Resources.TXT_QC_6_MONTH_SECTION_3_QUESTION_1, qcReport6Month.Section3Question1, 61, 575, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_6_MONTH_SECTION_3_QUESTION_2, qcReport6Month.Section3Question1, 61, 550, 480, 25);

            AbsolutePositionText(document, Resources.TXT_QC_6_MONTH_SECTION_4, 61, 525, 480, 25, document.GetRegularFont(true, true), Element.ALIGN_LEFT);
            AddAnswerSection(document, Resources.TXT_QC_6_MONTH_SECTION_4_QUESTION_1, qcReport6Month.Section4Question1, 61, 500, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_6_MONTH_SECTION_4_QUESTION_2, qcReport6Month.Section4Question2, 61, 475, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_6_MONTH_SECTION_4_QUESTION_3, qcReport6Month.Section4Question3, 61, 450, 480, 25);

            AbsolutePositionText(document, Resources.TXT_QC_6_MONTH_SECTION_5, 61, 425, 480, 25, document.GetRegularFont(true, true), Element.ALIGN_LEFT);
            AddAnswerSection(document, Resources.TXT_QC_6_MONTH_SECTION_5_QUESTION_1, qcReport6Month.Section5Question1, 61, 400, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_6_MONTH_SECTION_5_QUESTION_2, qcReport6Month.Section5Question1, 61, 375, 480, 25);

            AbsolutePositionText(document, Resources.TXT_QC_6_MONTH_SECTION_7, 61, 350, 480, 25, document.GetRegularFont(true, true), Element.ALIGN_LEFT);
            AddAnswerSection(document, Resources.TXT_QC_6_MONTH_SECTION_7_QUESTION_1, qcReport6Month.Section7Question1, 61, 325, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_6_MONTH_SECTION_7_QUESTION_2, qcReport6Month.Section7Question1, 61, 300, 480, 25);
            
            AbsolutePositionText(document, Resources.TXT_QC_6_MONTH_SECTION_9, 61, 275, 480, 25, document.GetRegularFont(true, true), Element.ALIGN_LEFT);
            AddAnswerSection(document, Resources.TXT_QC_6_MONTH_SECTION_9_10_QUESTION_1, qcReport6Month.CalibrationSection9And10Question1, 61, 250, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_6_MONTH_SECTION_9_10_QUESTION_2, qcReport6Month.CalibrationSection9And10Question2, 61, 225, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_6_MONTH_SECTION_9_10_QUESTION_3, qcReport6Month.CalibrationSection9And10Question3, 61, 200, 480, 25);

            AbsolutePositionText(document, Resources.TXT_QC_6_MONTH_SECTION_10, 61, 175, 480, 25, document.GetRegularFont(true, true), Element.ALIGN_LEFT);
            AddAnswerSection(document, Resources.TXT_QC_6_MONTH_SECTION_10_QUESTION_1, qcReport6Month.DataManagementSection10Question1, 61, 150, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_6_MONTH_SECTION_10_QUESTION_2, qcReport6Month.DataManagementSection10Question2, 61, 125, 480, 25);
            AddAnswerSection(document, Resources.TXT_QC_6_MONTH_SECTION_10_QUESTION_3, qcReport6Month.DataManagementSection10Question3, 61, 100, 480, 25);

            document.AddPage();

            AbsolutePositionText(document, Resources.TXT_QC_6_MONTH_STATE_REASONS, 61, 767, 500, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);
            document.DrawBox(61, 525, 464, 200);
            AbsolutePositionText(document, qcReport6Month.FurtherDetails, 71, 615, 480, 730, document.GetRegularFont(false), Element.ALIGN_LEFT);

            document.DrawBox(61, 450, 225, 35);
            AbsolutePositionText(document, $"{Resources.TXT_QC_REPORT_CHECK_TECHNICIAN} {qcReport6Month.Name}", 65, 480, 255, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);

            document.DrawBox(61, 400, 225, 35);
            AbsolutePositionText(document, Resources.TXT_QC_REPORT_CHECK_SIGNATURE, 65, 430, 450, 25, document.GetRegularFont(false), Element.ALIGN_LEFT);
            TryAddSignature(document, qcReport6Month.Name, 100, 390);

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
            document.AddParagraph(text, absoluteColumn, font, BaseColor.BLACK, alignment);
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
