namespace TachographReader.Library.PDF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Connect.Shared.Models;
    using Core;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using Shared;
    using Document = Connect.Shared.Models.Document;

    public class CustomReport
    {
        private static PDFDocument _pdfDocument;

        public static void Create(PDFDocument pdfDocument, Report report)
        {
            if (report == null)
            {
                return;
            }

            _pdfDocument = pdfDocument;

            const int itemsPerPage = 20;
            var documents = CustomReportDataBuilder.GetAllDocuments(report).ToList();
            int pages = GetPageCount(documents.Count, itemsPerPage);

            for (int i = 0; i < pages; i++)
            {
                AddPage(i);

                CreateHeader();
                CreateDataTable(documents.Skip(i * itemsPerPage).Take(itemsPerPage).ToList());
            }
        }

        private static void CreateHeader()
        {
            ColumnText column1 = _pdfDocument.GetNewColumn(50, _pdfDocument.Height - 17, 200, 100);
            _pdfDocument.AddParagraph("Tachograph Report", column1, _pdfDocument.GetLargeFont(true));
        }

        private static void CreateDataTable(IList<Document> documents)
        {
            var table = new PdfPTable(7);

            table.SetWidths(new float[] { 165, 165, 165, 165, 165, 165, 165 });

            _pdfDocument.AddCell(table, "Document Type");
            _pdfDocument.AddCell(table, "Expiration");
            _pdfDocument.AddCell(table, "Registration Number");
            _pdfDocument.AddCell(table, "Technician");
            _pdfDocument.AddCell(table, "Customer");
            _pdfDocument.AddCell(table, "Vehicle Manufacturer");
            _pdfDocument.AddCell(table, "Invoice Number");

            for (int j = 1; j < 19; j++)
            {
                if (j < documents.Count + 1)
                {
                    Document document = documents[j - 1];
                    AddCell(table, document.GetType().Name.SplitByCapitals());
                    AddCell(table, document.Created.AddYears(2).ToString(Constants.DateFormat));
                    AddCell(table, document.RegistrationNumber);
                    AddCell(table, document.Technician);
                    AddCell(table, document.CustomerContact);

                    var tachographDocument = document as TachographDocument;
                    if (tachographDocument != null)
                    {
                        AddCell(table, tachographDocument.VehicleMake);
                        AddCell(table, tachographDocument.InvoiceNumber);
                    }
                    else
                    {
                        AddCell(table, string.Empty);
                        AddCell(table, string.Empty);
                    }
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
            table.WriteSelectedRows(0, -1, _pdfDocument.Document.LeftMargin + 10, _pdfDocument.Height - 160, _pdfDocument.ContentByte);
        }

        private static void AddCell(PdfPTable table, string text)
        {
            var cell = new PdfPCell(new Phrase(text, _pdfDocument.GetXSmallFont(false)));
            cell.MinimumHeight = 22;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);
        }

        private static void AddPage(int iteration)
        {
            if (iteration == 0)
            {
                return;
            }

            _pdfDocument.AddPage();
        }

        private static int GetPageCount(int documentCount, int itemsPerPage)
        {
            if (documentCount <= itemsPerPage)
            {
                return 1;
            }

            return (documentCount / itemsPerPage) + (documentCount % itemsPerPage > 0 ? 1 : 0);
        }
    }
}