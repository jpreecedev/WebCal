namespace TachographReader.Library.PDF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Resources;
    using Connect.Shared.Models;
    using Core;
    using DataModel;
    using DataModel.Core;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using Properties;
    using Shared;
    using Shared.Helpers;

    public static class GV212Document
    {
        public static PDFDocument Document { get; set; }

        public static void Create(string path, ICollection<TachographDocument> documents, DateTime start, DateTime end)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            const int itemsPerPage = 18;
            int pages = GetPageCount(documents.Count, itemsPerPage);

            using (Document = new PDFDocument(path))
            {
                for (int i = 0; i < pages; i++)
                {
                    AddPage(i);

                    CreateHeader(start, end);
                    CreateDataTable(documents.Skip(i*itemsPerPage).Take(itemsPerPage).ToList());
                    CreateFooter();
                }
            }
        }

        private static void CreateHeader(DateTime start, DateTime end)
        {
            RegistrationData registrationData = GetRegistrationData();

            ColumnText column1 = Document.GetNewColumn(50, Document.Height - 17, 100, 100);
            Document.AddParagraph(Resources.TXT_GV_212, column1, Document.GetLargeFont(true));

            //VOSA Logo
            var rawData = ImageHelper.LoadFromResourcesAsByteArray("Vosa");
            Document.AddImage(rawData, 100, 45, Document.Width - 115, Document.Height - 60);

            var column2 = new ColumnText(Document.ContentByte);
            column2.SetSimpleColumn(50, Document.Height - 50, 500, 100);

            Document.AddParagraph(Resources.TXT_REGISTER_OF_TACHOGRAPH_PLAQUES_ISSUES, column2, Document.GetLargeFont(true));
            Document.DrawLine(50, Document.Top - 38, 540, Document.Top - 38);

            ColumnText column3 = Document.GetNewColumn(50, Document.Height - 80, 200, 100);
            Document.AddParagraph(Resources.TXT_NAME_OF_TACHOGRAPH_CENTER, column3);
            Document.DrawLine(200, Document.Top - 63, 540, Document.Top - 63);

            //Tachograph Centre Name
            ColumnText centreNameColumn = Document.GetNewColumn(210, Document.Height - 80, 600, 100);
            Document.AddSmallParagraph(GetCompanyName(registrationData), centreNameColumn);

            ColumnText column4 = Document.GetNewColumn(50, Document.Height - 105, 220, 100);
            Document.AddParagraph(Resources.TXT_SEAL_OF_TACHOGRAPH_CENTER, column4);
            Document.DrawLine(220, Document.Top - 87, 320, Document.Top - 87);

            //Seal number
            ColumnText sealNumberColumn = Document.GetNewColumn(230, Document.Height - 104, 300, 100);
            Document.AddSmallParagraph(registrationData.SealNumber, sealNumberColumn);

            ColumnText column5 = Document.GetNewColumn(50, Document.Height - 130, 435, 100);
            Document.AddParagraph(Resources.TXT_REGISTER_BETWEEN, column5);
            Document.DrawLine(145, Document.Top - 111, 220, Document.Top - 111);

            //Register Start Date
            ColumnText startColumn = Document.GetNewColumn(145, Document.Height - 128, 220, 100);
            Document.AddSmallParagraph(start.ToString(Constants.DateFormat), startColumn);

            ColumnText column6 = Document.GetNewColumn(230, Document.Height - 130, 435, 100);
            Document.AddParagraph(Resources.TXT_AND, column6);
            Document.DrawLine(258, Document.Top - 111, 344, Document.Top - 111);

            //Register end date
            ColumnText endColumn = Document.GetNewColumn(258, Document.Height - 128, 350, 100);
            Document.AddSmallParagraph(end.ToString(Constants.DateFormat), endColumn);
        }

        private static string GetCompanyName(RegistrationData registrationData)
        {
            if (registrationData.HasValidLicense)
            {
                return registrationData.CompanyName;
            }

            return Resources.TXT_SKILLRAY_UNLICENSED_SOFTWARE;
        }

        private static void CreateDataTable(IList<TachographDocument> documents)
        {
            var table = new PdfPTable(10);

            table.SetWidths(new float[] {170, 100, 100, 130, 100, 100, 100, 100, 110, 150});

            Document.AddCell(table, Resources.TXT_VOSA_REGISTRATION_HEADER);
            Document.AddCell(table, Resources.TXT_VOSA_VEHICLE_TYPE_HEADER);
            Document.AddSpannedCell(table, Resources.TXT_VOSA_TACHOGRAPH_DETAILS_HEADER, 2);
            Document.AddCell(table, Resources.TXT_VOSA_TYPE_INSPECTION_HEADER);
            Document.AddCell(table, Resources.TXT_VOSA_TURNS_HEADER);
            Document.AddCell(table, Resources.TXT_VOSA_L_HEADER);
            Document.AddCell(table, Resources.TXT_VOSA_DATE_HEADER);
            Document.AddCell(table, Resources.TXT_VOSA_TECH_INITIALS_HEADER);
            Document.AddCell(table, Resources.TXT_VOSA_INVOICE_NUMBER_HEADER);

            for (int j = 1; j < 19; j++)
            {
                if (j < documents.Count + 1)
                {
                    TachographDocument tachograph = documents[j - 1];


                    BaseColor backgroundColor = null;
                    if (tachograph.IsQCCheck)
                    {
                        backgroundColor = new BaseColor(255, 255, 224);
                    }

                    AddCell(table, $"{tachograph.RegistrationNumber} / \n{tachograph.VIN}", backgroundColor);
                    AddCell(table, tachograph.VehicleType.Substring(0, 1).ToUpper(), backgroundColor);

                    string type = tachograph.IsDigital ? Resources.TXT_DIGITAL : Resources.TXT_ANALOGUE;
                    if (tachograph.Tampered)
                    {
                        type = type + Resources.TXT_STAR;
                    }
                    AddCell(table, type, backgroundColor);

                    AddCell(table, tachograph.SerialNumber, backgroundColor);
                    AddCell(table, tachograph.DocumentType.Substring(0, 1).ToUpper(), backgroundColor);
                    AddCell(table, tachograph.WFactor, backgroundColor);
                    AddCell(table, tachograph.LFactor, backgroundColor);
                    AddCell(table, GetCalibrationTime(tachograph.CalibrationTime), backgroundColor);
                    AddCell(table, tachograph.IsDigital ? tachograph.CardSerialNumber : GetTechnicianInitials(tachograph.Technician), backgroundColor);
                    AddCell(table, tachograph.InvoiceNumber, backgroundColor);
                }
                else
                {
                    for (int i = 0; i < 11; i++)
                    {
                        AddCell(table, String.Empty);
                    }
                }
            }

            table.TotalWidth = 520;
            table.WriteSelectedRows(0, -1, Document.Document.LeftMargin + 10, Document.Height - 160, Document.ContentByte);
        }

        private static string GetCalibrationTime(DateTime? calibrationTime)
        {
            if (calibrationTime == null)
            {
                return DateTime.Now.ToString("yyyy-MM-dd");
            }

            return calibrationTime.Value.ToString("yyyy-MM-dd");
        }

        private static void AddCell(PdfPTable table, string text, BaseColor backgroundColor = null)
        {
            var cell = new PdfPCell(new Phrase(text, Document.GetXSmallFont(false)));
            cell.MinimumHeight = 22;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;

            if (backgroundColor != null)
            {
                cell.BackgroundColor = backgroundColor;
            }
            else
            {
                cell.BackgroundColor = new BaseColor(255,255,255);
            }

            table.AddCell(cell);
        }

        private static void CreateFooter()
        {
            var rawData = ImageHelper.LoadFromResourcesAsByteArray("Footer");
            Document.AddImage(rawData, 525, 171, Document.MarginLeft + 8, 20);
        }

        private static RegistrationData GetRegistrationData()
        {
            return ContainerBootstrapper.Resolve<IRepository<RegistrationData>>().GetAll().First();
        }

        private static string GetTechnicianInitials(string technicianName)
        {
            if (string.IsNullOrEmpty(technicianName))
            {
                return string.Empty;
            }

            string[] split = technicianName.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            return split.Aggregate(string.Empty, (current, s) => current + s.Substring(0, 1).ToUpper());
        }

        private static void AddPage(int iteration)
        {
            if (iteration == 0)
            {
                return;
            }

            Document.AddPage();
        }

        private static int GetPageCount(int documentCount, int itemsPerPage)
        {
            if (documentCount <= itemsPerPage)
            {
                return 1;
            }

            return (documentCount/itemsPerPage) + (documentCount%itemsPerPage > 0 ? 1 : 0);
        }
    }
}