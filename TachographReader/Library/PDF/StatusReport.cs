namespace TachographReader.Library.PDF
{
    using System;
    using System.Collections.Generic;
    using Core;
    using DataModel;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using Shared.Helpers;
    using ViewModels;

    public static class StatusReport
    {
        public static void Create(PDFDocument document, StatusReportViewModel statusReport)
        {
            CreateQuarterlyCheckTable(document, statusReport);
            CreateMonthlyGV212Table(document, statusReport);
            CreateTechniciansQuarterlyTable(document, document.Height - 160, statusReport.Technicians);
            CreateTechnicians3YearTable(document, document.Height - 160, statusReport.Technicians);
        }

        private static void CreateTechniciansQuarterlyTable(PDFDocument document, float top, IEnumerable<Technician> technicians)
        {
            document.AddPage();
            AddImageFromResource(document, "skillray_small", 61, 770);
            AddImageFromResource(document, "webcal_print_logo", 300, 770);

            AbsolutePositionText(document, "Technicians Qu Report", 61, document.Height - 120, 300,35, document.GetLargeFont(true), Element.ALIGN_LEFT);

            var table = new PdfPTable(3);
            table.SetWidths(new float[] { 386, 386, 386 });

            AddCell(document, table, "Technician name", BaseColor.BLACK, true);
            AddCell(document, table, "Date of next check", BaseColor.BLACK, true);
            AddCell(document, table, "Status", BaseColor.BLACK, true);

            foreach (var technician in technicians)
            {
                string statusText;
                var color = GetStatus(GetTechnicianStatus(technician), out statusText);

                AddCell(document, table, technician.Name, color, false);
                AddCell(document, table, technician.DateOfLastCheck == null ? string.Empty : technician.DateOfLastCheck.Value.ToString(Constants.ShortYearDateFormat), color, false);
                AddCell(document, table, statusText, BaseColor.BLACK, false);
            }

            table.TotalWidth = 520;
            table.WriteSelectedRows(0, -1, document.Document.LeftMargin + 10, top, document.ContentByte);
        }

        private static void CreateTechnicians3YearTable(PDFDocument document, float top, IEnumerable<Technician> technicians)
        {
            document.AddPage();
            AddImageFromResource(document, "skillray_small", 61, 770);
            AddImageFromResource(document, "webcal_print_logo", 300, 770);

            AbsolutePositionText(document, "Technician Training (3 yearly)", 61, document.Height - 120, 300, 35, document.GetLargeFont(true), Element.ALIGN_LEFT);

            var table = new PdfPTable(3);
            table.SetWidths(new float[] { 386, 386, 386 });

            AddCell(document, table, "Technician name", BaseColor.BLACK, true);
            AddCell(document, table, "Date of next check", BaseColor.BLACK, true);
            AddCell(document, table, "Status", BaseColor.BLACK, true);

            foreach (var technician in technicians)
            {
                string statusText;
                var color = GetStatus(GetTechnicianStatus(technician), out statusText);

                AddCell(document, table, technician.Name, color, false);
                AddCell(document, table, technician.DateOfLastCheck == null ? string.Empty : technician.DateOfLastCheck.Value.ToString(Constants.ShortYearDateFormat), color, false);
                AddCell(document, table, statusText, color, false);
            }

            table.TotalWidth = 520;
            table.WriteSelectedRows(0, -1, document.Document.LeftMargin + 10, top, document.ContentByte);
        }

        private static void CreateQuarterlyCheckTable(PDFDocument document, StatusReportViewModel statusReport)
        {
            AddImageFromResource(document, "skillray_small", 61, 770);
            AddImageFromResource(document, "webcal_print_logo", 300, 770);

            AbsolutePositionText(document, "Centre Quarterly Check", 61, document.Height - 120, 300, 35, document.GetLargeFont(true), Element.ALIGN_LEFT);
            
            var table = new PdfPTable(3);
            table.SetWidths(new float[] { 386, 386, 386 });

            AddCell(document, table, "Date of last check", BaseColor.BLACK, true);
            AddCell(document, table, "Date of next check", BaseColor.BLACK, true);
            AddCell(document, table, "Status", BaseColor.BLACK, true);

            string statusText;
            var color = GetStatus(GetTachoCentreStatus(statusReport), out statusText);
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
            table.SetWidths(new float[] { 579, 579 });

            AbsolutePositionText(document, "Monthly GV212", 61, document.Height - 220, 300, 35, document.GetLargeFont(true), Element.ALIGN_LEFT);

            AddCell(document, table, "Generated and printed", BaseColor.BLACK, true);
            AddCell(document, table, "Status", BaseColor.BLACK, true);

            string statusText;
            var color = GetStatus(GetGV212Status(statusReport), out statusText);

            AddCell(document, table, statusReport.GV212LastCheck == null ? string.Empty : statusReport.GV212LastCheck.Value.ToString(Constants.ShortYearDateFormat), color, false);
            AddCell(document, table, statusText, color, false);

            table.TotalWidth = 520;
            table.WriteSelectedRows(0, -1, document.Document.LeftMargin + 10, document.Height - 260, document.ContentByte);
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

        private static Status GetTechnicianStatus(Technician technician)
        {
            if (technician == null)
            {
                return Status.Unknown;
            }

            var lastCheck = technician.DateOfLastCheck.GetValueOrDefault().Date;
            if (lastCheck == default(DateTime))
            {
                return Status.Unknown;
            }

            var earliest = DateTime.Now.Date.AddMonths(-3);
            var checkDue = DateTime.Now.Date.AddDays(-7);

            if (lastCheck >= checkDue)
            {
                return Status.CheckDue;
            }
            if (lastCheck >= earliest && lastCheck < checkDue)
            {
                return Status.Ok;
            }
            if (lastCheck < earliest)
            {
                return Status.Expired;
            }

            return Status.Unknown;
        }

        private static Status GetTachoCentreStatus(StatusReportViewModel statusReport)
        {
            var lastCheck = statusReport.TachoCentreLastCheck.GetValueOrDefault();
            if (lastCheck == default(DateTime))
            {
                return Status.Unknown;
            }

            var nextCheck = lastCheck.AddMonths(3).Date;
            var dueDate = nextCheck.AddDays(-7);
            if (DateTime.Now.Date > nextCheck)
            {
                return Status.Expired;
            }
            if (DateTime.Now.Date >= dueDate)
            {
                return Status.CheckDue;
            }
            if (DateTime.Now.Date < dueDate)
            {
                return Status.Ok;
            }
            return Status.Unknown;
        }

        private static Status GetGV212Status(StatusReportViewModel statusReport)
        {
            var lastCheck = statusReport.GV212LastCheck.GetValueOrDefault();
            if (lastCheck == default(DateTime))
            {
                return Status.Unknown;
            }

            var checkMonth = lastCheck.Date.Month;
            var checkYear = lastCheck.Date.Year;
            if (checkMonth != DateTime.Now.Month && checkYear != DateTime.Now.Year)
            {
                return Status.Expired;
            }
            return Status.Ok;
        }

        private static BaseColor GetStatus(Status status, out string statusText)
        {
            BaseColor color;
            switch (status)
            {
                case Status.Ok:
                    color = new BaseColor(0, 100, 0); 
                    statusText = "OK";
                    break;
                case Status.CheckDue:
                    color = new BaseColor(255, 140, 0);
                    statusText = "Check due";
                    break;
                case Status.Expired:
                    color = new BaseColor(178, 34, 34);
                    statusText = "Expired";
                    break;
                default:
                    color = new BaseColor(178, 34, 34);
                    statusText = "Unknown";
                    break;
            }
            return color;
        }

        private enum Status
        {
            Ok,
            CheckDue,
            Expired,
            Unknown
        }
    }
}