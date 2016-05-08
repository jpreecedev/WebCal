namespace TachographReader.Library.PDF
{
    using System.Collections.Generic;
    using Connect.Shared;
    using Connect.Shared.Models;
    using Core;
    using DataModel;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using Properties;
    using Shared.Helpers;
    using ViewModels;

    public static class StatusReport
    {
        public static void Create(PDFDocument document, StatusReportViewModel statusReport)
        {
            CreateTachoCentreQuarterlyCheckTable(document, statusReport);
            CreateMonthlyGV212Table(document, statusReport);
            CreateTechniciansQuarterlyTable(document, statusReport.Technicians);
        }

        private static void CreateTachoCentreQuarterlyCheckTable(PDFDocument document, StatusReportViewModel statusReport)
        {
            AddImageFromResource(document, "skillray_small", 61, 770);
            AddImageFromResource(document, "webcal_print_logo", 325, 770);

            AbsolutePositionText(document, Resources.TXT_STATUS_REPORT_CENTRE_QUARTERLY_CHECK, 61, document.Height - 80, 300, 35, document.GetRegularFont(true, true), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_STATUS_REPORT_DATE_OF_LAST_CHECK, 61, document.Height - 100, 261, 35, document.GetRegularFont(true), Element.ALIGN_LEFT);
            AbsolutePositionText(document, Resources.TXT_STATUS_REPORT_DATE_OF_NEXT_CHECK, 141, document.Height - 100, 341, 35, document.GetRegularFont(true), Element.ALIGN_LEFT);
            AbsolutePositionText(document, Resources.TXT_STATUS_REPORT_STATUS, 221, document.Height - 100, 421, 35, document.GetRegularFont(true), Element.ALIGN_LEFT);

            string statusText;
            var color = GetStatus(statusReport.TachoCentreQuarterlyStatus, out statusText);

            if (statusReport.TachoCentreLastCheck != null)
            {
                var lastCheck = statusReport.TachoCentreLastCheck.GetValueOrDefault();
                var nextCheck = lastCheck.AddMonths(3).Date;
                AbsolutePositionText(document, lastCheck.ToString(Constants.ShortYearDateFormat), 61, document.Height - 120, 261, 35, document.GetRegularFont(false, color), Element.ALIGN_LEFT);
                AbsolutePositionText(document, nextCheck.ToString(Constants.ShortYearDateFormat), 141, document.Height - 120, 341, 35, document.GetRegularFont(false, color), Element.ALIGN_LEFT);
            }

            AbsolutePositionText(document, statusText, 661, document.Height - 120, 221, 35, document.GetRegularFont(false, color), Element.ALIGN_LEFT);
        }

        private static void CreateMonthlyGV212Table(PDFDocument document, StatusReportViewModel statusReport)
        {
            AbsolutePositionText(document, Resources.TXT_STATUS_REPORT_MONTHLY_GV_212, 325, document.Height - 80, 425, 35, document.GetRegularFont(true, true), Element.ALIGN_LEFT);

            AbsolutePositionText(document, Resources.TXT_STATUS_REPORT_GENERATED_AND_PRINTED, 325, document.Height - 100, 425, 35, document.GetRegularFont(true), Element.ALIGN_LEFT);
            AbsolutePositionText(document, Resources.TXT_STATUS_REPORT_STATUS, 450, document.Height - 100, 550, 35, document.GetRegularFont(true), Element.ALIGN_LEFT);

            string statusText;
            var color = GetStatus(statusReport.GV212Status, out statusText);

            AbsolutePositionText(document, statusReport.GV212LastCheck == null ? string.Empty : statusReport.GV212LastCheck.Value.ToString(Constants.ShortYearDateFormat), 325, document.Height - 120, 500, 35, document.GetRegularFont(false, color), Element.ALIGN_LEFT);
            AbsolutePositionText(document, statusText, 450, document.Height - 120, 625, 35, document.GetRegularFont(false, color), Element.ALIGN_LEFT);
        }

        private static void CreateTechniciansQuarterlyTable(PDFDocument document, IEnumerable<Technician> technicians)
        {
            AbsolutePositionText(document, Resources.TXT_STATUS_REPORT_TECHNICIANS_QU_REPORT, 61, document.Height - 160, 300, 35, document.GetRegularFont(true, true), Element.ALIGN_LEFT);

            var table = new PdfPTable(4);
            table.SetWidths(new float[] {289, 289, 289, 289});

            AddCell(document, table, Resources.TXT_STATUS_REPORT_TECHNICIAN_NAME, BaseColor.BLACK, true);
            AddCell(document, table, Resources.TXT_STATUS_REPORT_DATE_OF_NEXT_CHECK, BaseColor.BLACK, true);
            AddCell(document, table, Resources.TXT_TRAINING_DATE, BaseColor.BLACK, true);
            AddCell(document, table, Resources.TXT_STATUS_REPORT_STATUS, BaseColor.BLACK, true);

            foreach (var technician in technicians)
            {
                string statusText;
                var color = GetStatus(technician.HalfYearStatus(), out statusText);

                string threeYearStatusText;
                var threeYearColor = GetStatus(technician.ThreeYearStatus(), out threeYearStatusText);

                AddCell(document, table, technician.Name, color, false);
                AddCell(document, table, technician.DateOfLastCheck == null ? string.Empty : technician.DateOfLastCheck.Value.ToString(Constants.ShortYearDateFormat), color, false);
                AddCell(document, table, technician.DateOfLast3YearCheck == null ? string.Empty : technician.DateOfLast3YearCheck.Value.ToString(Constants.ShortYearDateFormat), threeYearColor, false);

                BaseColor statusTextColor;
                AddCell(document, table, GetStatusText(statusText, threeYearStatusText, out statusTextColor), statusTextColor, false);
            }

            table.TotalWidth = 520;
            table.WriteSelectedRows(0, -1, document.Document.LeftMargin + 10, document.Height - 190, document.ContentByte);
        }

        private static void AddCell(PDFDocument document, PdfPTable table, string text, BaseColor color, bool bold)
        {
            var font = document.GetRegularFont(bold);
            font.Color = color;

            var cell = new PdfPCell(new Phrase(text, font))
            {
                MinimumHeight = 22,
                HorizontalAlignment = Element.ALIGN_CENTER
            };

            table.AddCell(cell);
        }

        private static void AddImageFromResource(PDFDocument document, string resourceName, int x, int y)
        {
            var image = ImageHelper.LoadFromResources(resourceName);
            var bitmap = image.ToBitmap();
            var scaled = ImageHelper.Scale(bitmap, 0, 25);
            document.AddImage(ImageHelper.ToByteArray(scaled), scaled.Width, scaled.Height, x, y);
        }

        private static void AbsolutePositionText(PDFDocument document, string text, float left, float top, float width, float height, Font font, int alignment)
        {
            var absoluteColumn = document.GetNewColumn(left, top, width, height);
            document.AddParagraph(text, absoluteColumn, font, font.Color, alignment);
        }
        
        private static string GetStatusText(string statusText, string threeYearStatusText, out BaseColor color)
        {
            if (statusText == Resources.TXT_STATUS_REPORT_ok && threeYearStatusText == Resources.TXT_STATUS_REPORT_ok)
            {
                color = new BaseColor(0, 100, 0);
                return Resources.TXT_STATUS_REPORT_ok;
            }
            if (statusText == Resources.TXT_STATUS_REPORT_CHECK_DUE && threeYearStatusText == Resources.TXT_STATUS_REPORT_CHECK_DUE)
            {
                color = new BaseColor(255, 140, 0);
                return Resources.TXT_STATUS_REPORT_CHECK_DUE;
            }
            if (statusText == Resources.TXT_STATUS_REPORT_EXPIRED && threeYearStatusText == Resources.TXT_STATUS_REPORT_EXPIRED)
            {
                color = new BaseColor(178, 34, 34);
                return Resources.TXT_STATUS_REPORT_EXPIRED;
            }
            if (statusText == Resources.TXT_STATUS_REPORT_UNKNOWN && threeYearStatusText == Resources.TXT_STATUS_REPORT_UNKNOWN)
            {
                color = new BaseColor(178, 34, 34);
                return Resources.TXT_STATUS_REPORT_UNKNOWN;
            }
            if (statusText == Resources.TXT_STATUS_REPORT_ok && threeYearStatusText == Resources.TXT_STATUS_REPORT_CHECK_DUE)
            {
                color = new BaseColor(255, 140, 0);
                return Resources.TXT_STATUS_REPORT_CHECK_DUE;
            }
            if (statusText == Resources.TXT_STATUS_REPORT_ok && threeYearStatusText == Resources.TXT_STATUS_REPORT_EXPIRED)
            {
                color = new BaseColor(178, 34, 34);
                return Resources.TXT_STATUS_REPORT_EXPIRED;
            }
            if (statusText == Resources.TXT_STATUS_REPORT_ok && threeYearStatusText == Resources.TXT_STATUS_REPORT_UNKNOWN)
            {
                color = new BaseColor(178, 34, 34);
                return Resources.TXT_STATUS_REPORT_UNKNOWN;
            }
            if (threeYearStatusText == Resources.TXT_STATUS_REPORT_ok && statusText == Resources.TXT_STATUS_REPORT_CHECK_DUE)
            {
                color = new BaseColor(255, 140, 0);
                return Resources.TXT_STATUS_REPORT_CHECK_DUE;
            }
            if (threeYearStatusText == Resources.TXT_STATUS_REPORT_ok && statusText == Resources.TXT_STATUS_REPORT_EXPIRED)
            {
                color = new BaseColor(178, 34, 34);
                return Resources.TXT_STATUS_REPORT_EXPIRED;
            }
            if (threeYearStatusText == Resources.TXT_STATUS_REPORT_ok && statusText == Resources.TXT_STATUS_REPORT_UNKNOWN)
            {
                color = new BaseColor(178, 34, 34);
                return Resources.TXT_STATUS_REPORT_UNKNOWN;
            }
            color = new BaseColor(0,0,0);
            return string.Empty;
        }

        private static BaseColor GetStatus(ReportItemStatus itemStatus, out string statusText)
        {
            BaseColor color;
            switch (itemStatus)
            {
                case ReportItemStatus.Ok:
                    color = new BaseColor(0, 100, 0);
                    statusText = Resources.TXT_STATUS_REPORT_ok;
                    break;
                case ReportItemStatus.CheckDue:
                    color = new BaseColor(255, 140, 0);
                    statusText = Resources.TXT_STATUS_REPORT_CHECK_DUE;
                    break;
                case ReportItemStatus.Expired:
                    color = new BaseColor(178, 34, 34);
                    statusText = Resources.TXT_STATUS_REPORT_EXPIRED;
                    break;
                default:
                    color = new BaseColor(178, 34, 34);
                    statusText = Resources.TXT_STATUS_REPORT_UNKNOWN;
                    break;
            }
            return color;
        }
    }
}