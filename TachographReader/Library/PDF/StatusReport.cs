namespace TachographReader.Library.PDF
{
    using System.Collections.Generic;
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
            CreateTechnicians3YearTable(document, statusReport.Technicians);
        }

        private static void CreateTachoCentreQuarterlyCheckTable(PDFDocument document, StatusReportViewModel statusReport)
        {
            AddLogos(document);
            AbsolutePositionText(document, Resources.TXT_STATUS_REPORT_CENTRE_QUARTERLY_CHECK, 61, document.Height - 120, 300, 35, document.GetLargeFont(true), Element.ALIGN_LEFT);

            var table = new PdfPTable(3);
            table.SetWidths(new float[] {386, 386, 386});

            AddCell(document, table, Resources.TXT_STATUS_REPORT_DATE_OF_LAST_CHECK, BaseColor.BLACK, true);
            AddCell(document, table, Resources.TXT_STATUS_REPORT_DATE_OF_NEXT_CHECK, BaseColor.BLACK, true);
            AddCell(document, table, Resources.TXT_STATUS_REPORT_STATUS, BaseColor.BLACK, true);

            string statusText;
            var color = GetStatus(statusReport.TachoCentreQuarterlyStatus, out statusText);
            var lastCheck = statusReport.TachoCentreLastCheck.GetValueOrDefault();
            var nextCheck = lastCheck.AddMonths(3).Date;

            AddCell(document, table, lastCheck.ToString(Constants.ShortYearDateFormat), color, false);
            AddCell(document, table, nextCheck.ToString(Constants.ShortYearDateFormat), color, false);
            AddCell(document, table, statusText, color, false);

            table.TotalWidth = 520;
            table.WriteSelectedRows(0, -1, document.Document.LeftMargin + 10, document.Height - 160, document.ContentByte);
        }

        private static void CreateMonthlyGV212Table(PDFDocument document, StatusReportViewModel statusReport)
        {
            var table = new PdfPTable(2);
            table.SetWidths(new float[] {579, 579});

            AbsolutePositionText(document, Resources.TXT_STATUS_REPORT_MONTHLY_GV_212, 61, document.Height - 220, 300, 35, document.GetLargeFont(true), Element.ALIGN_LEFT);

            AddCell(document, table, Resources.TXT_STATUS_REPORT_GENERATED_AND_PRINTED, BaseColor.BLACK, true);
            AddCell(document, table, Resources.TXT_STATUS_REPORT_STATUS, BaseColor.BLACK, true);

            string statusText;
            var color = GetStatus(statusReport.GV212Status, out statusText);

            AddCell(document, table, statusReport.GV212LastCheck == null ? string.Empty : statusReport.GV212LastCheck.Value.ToString(Constants.ShortYearDateFormat), color, false);
            AddCell(document, table, statusText, color, false);

            table.TotalWidth = 520;
            table.WriteSelectedRows(0, -1, document.Document.LeftMargin + 10, document.Height - 260, document.ContentByte);
        }

        private static void CreateTechniciansQuarterlyTable(PDFDocument document, IEnumerable<Technician> technicians)
        {
            document.AddPage();
            AddLogos(document);

            AbsolutePositionText(document, Resources.TXT_STATUS_REPORT_TECHNICIANS_QU_REPORT, 61, document.Height - 120, 300, 35, document.GetLargeFont(true), Element.ALIGN_LEFT);

            var table = new PdfPTable(3);
            table.SetWidths(new float[] {386, 386, 386});

            AddCell(document, table, Resources.TXT_STATUS_REPORT_TECHNICIAN_NAME, BaseColor.BLACK, true);
            AddCell(document, table, Resources.TXT_STATUS_REPORT_DATE_OF_NEXT_CHECK, BaseColor.BLACK, true);
            AddCell(document, table, Resources.TXT_STATUS_REPORT_STATUS, BaseColor.BLACK, true);

            foreach (var technician in technicians)
            {
                string statusText;
                var color = GetStatus(technician.QuarterlyStatus(), out statusText);

                AddCell(document, table, technician.Name, color, false);
                AddCell(document, table, technician.DateOfLastCheck == null ? string.Empty : technician.DateOfLastCheck.Value.ToString(Constants.ShortYearDateFormat), color, false);
                AddCell(document, table, statusText, color, false);
            }

            table.TotalWidth = 520;
            table.WriteSelectedRows(0, -1, document.Document.LeftMargin + 10, document.Height - 160, document.ContentByte);
        }

        private static void CreateTechnicians3YearTable(PDFDocument document, IEnumerable<Technician> technicians)
        {
            document.AddPage();
            AddLogos(document);

            AbsolutePositionText(document, Resources.TXT_STATUS_REPORT_TECHNICIAN_TRAINING_3_YEARLY, 61, document.Height - 120, 300, 35, document.GetLargeFont(true), Element.ALIGN_LEFT);

            var table = new PdfPTable(3);
            table.SetWidths(new float[] {386, 386, 386});

            AddCell(document, table, Resources.TXT_STATUS_REPORT_TECHNICIAN_NAME, BaseColor.BLACK, true);
            AddCell(document, table, Resources.TXT_STATUS_REPORT_DATE_OF_NEXT_CHECK, BaseColor.BLACK, true);
            AddCell(document, table, Resources.TXT_STATUS_REPORT_STATUS, BaseColor.BLACK, true);

            foreach (var technician in technicians)
            {
                string statusText;
                var color = GetStatus(technician.ThreeYearStatus(), out statusText);

                AddCell(document, table, technician.Name, color, false);
                AddCell(document, table, technician.DateOfLastCheck == null ? string.Empty : technician.DateOfLastCheck.Value.ToString(Constants.ShortYearDateFormat), color, false);
                AddCell(document, table, statusText, color, false);
            }

            table.TotalWidth = 520;
            table.WriteSelectedRows(0, -1, document.Document.LeftMargin + 10, document.Height - 160, document.ContentByte);
        }

        private static void AddLogos(PDFDocument document)
        {
            AddImageFromResource(document, "skillray_small", 61, 770);
            AddImageFromResource(document, "webcal_print_logo", 300, 770);
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
            document.AddImage(scaled.ToByteArray(), scaled.Width, scaled.Height, x, y);
        }

        private static void AbsolutePositionText(PDFDocument document, string text, float left, float top, float width, float height, Font font, int alignment)
        {
            var absoluteColumn = document.GetNewColumn(left, top, width, height);
            document.AddParagraph(text, absoluteColumn, font, alignment);
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