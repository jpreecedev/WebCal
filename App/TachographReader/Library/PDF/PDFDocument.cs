namespace Webcal.Library.PDF
{
    using System;
    using System.IO;
    using iTextSharp.text;
    using iTextSharp.text.pdf;

    public class PDFDocument : IDisposable
    {
        public PDFDocument(string path)
        {
            Document = new Document();

            Writer = PdfWriter.GetInstance(Document, new FileStream(path, FileMode.Create));
            Document.Open();
        }

        public PlaqueDocumentType PlaqueDocumentType { get; set; }

        public Document Document { get; set; }

        public PdfWriter Writer { get; set; }

        public float Height
        {
            get { return Document.PageSize.Height; }
        }

        public float Width
        {
            get { return Document.PageSize.Width; }
        }

        public float Top
        {
            get { return Document.Top; }
        }

        public float MarginLeft
        {
            get { return Document.LeftMargin; }
        }

        public PdfContentByte ContentByte
        {
            get { return Writer.DirectContent; }
        }

        public void Dispose()
        {
            Document.Close();
        }

        public ColumnText GetNewColumn(float left, float top, float width, float height)
        {
            var column = new ColumnText(ContentByte);
            column.SetSimpleColumn(left, top, width, height);
            return column;
        }

        public void DrawLine(float x1, float y1, float x2, float y2)
        {
            ContentByte.SetLineWidth(1.0f);
            ContentByte.MoveTo(x1, y1);
            ContentByte.LineTo(x2, y2);
            ContentByte.Stroke();
        }

        public void DrawLine(float x1, float y1, float x2, float y2, int pageHeight)
        {
            ContentByte.SetLineWidth(1.0f);
            ContentByte.MoveTo(x1, (pageHeight - y1));
            ContentByte.LineTo(x2, (pageHeight - y2));
            ContentByte.Stroke();
        }

        public void DrawCheckBox(float x1, float y1, int pageHeight)
        {
            ContentByte.SetLineWidth(1.0f);
            ContentByte.MoveTo(x1, (pageHeight - y1));
            ContentByte.LineTo(x1 + 7, (pageHeight - y1));
            ContentByte.LineTo(x1 + 7, (pageHeight - y1) - 7);
            ContentByte.LineTo(x1, (pageHeight - y1) - 7);
            ContentByte.LineTo(x1, (pageHeight - y1));
        }

        public void DrawCheck(float x1, float y1, int pageHeight)
        {
            ContentByte.SetLineWidth(1.0f);
            ContentByte.MoveTo(x1, (pageHeight - y1));
            ContentByte.LineTo(x1 + 7, (pageHeight - y1) - 7);
            ContentByte.MoveTo(x1 + 7, (pageHeight - y1));
            ContentByte.LineTo(x1, (pageHeight - y1) - 7);
        }

        public void AddImage(byte[] rawImage, float width, float height, float x, float y)
        {
            Image image = Image.GetInstance(rawImage);
            image.ScaleAbsolute(width, height);
            image.SetAbsolutePosition(x, y);

            Document.Add(image);
        }

        public void AddImage(string imagePath, float width, float height, float x, float y)
        {
            Image image = Image.GetInstance(imagePath);
            image.ScaleAbsolute(width, height);
            image.SetAbsolutePosition(x, y);

            Document.Add(image);
        }

        public Image GetImage(string imagePath, float width, float height)
        {
            Image image = Image.GetInstance(imagePath);
            image.ScaleAbsolute(width, height);
            return image;
        }

        public void AddSmallParagraph(string text, ColumnText column)
        {
            var para = new Paragraph(text, GetSmallFont(false));
            column.AddText(para);
            column.Go();
        }

        public void AddParagraph(string text, ColumnText column)
        {
            var para = new Paragraph(text, GetRegularFont(false));
            column.AddText(para);
            column.Go();
        }

        public void AddParagraph(string text, ColumnText column, Font font)
        {
            var para = new Paragraph(text, font);
            column.AddText(para);
            column.Go();
        }

        public void AddParagraph(string text, ColumnText column, Font font, int alignment)
        {
            var para = new Paragraph(text, font);
            para.Alignment = alignment;
            column.AddText(para);
            column.Go();
        }

        public void AddCell(PdfPTable table, string text)
        {
            var paragraph = new Paragraph(text, GetSmallFont(false)) {Alignment = Element.ALIGN_CENTER};

            var cell = new PdfPCell(paragraph);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_CENTER;

            table.AddCell(cell);
        }

        public void AddCell(PdfPTable table, string text, Font font)
        {
            var paragraph = new Paragraph(text, font) {Alignment = Element.ALIGN_CENTER};

            var cell = new PdfPCell(paragraph);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_CENTER;

            table.AddCell(cell);
        }

        public void AddCell(PdfPTable table, string text, Font font, int height)
        {
            var paragraph = new Paragraph(text, font) {Alignment = Element.ALIGN_CENTER};

            var cell = new PdfPCell(paragraph);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.FixedHeight = height;

            table.AddCell(cell);
        }

        public void AddCell(PdfPTable table, string text, Font font, int height, int width)
        {
            var paragraph = new Paragraph(text, font) {Alignment = Element.ALIGN_CENTER};

            var cell = new PdfPCell(paragraph);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.FixedHeight = height;

            table.AddCell(cell);
        }

        public void AddSpannedCell(PdfPTable table, string text, int colspan)
        {
            var c = new PdfPCell(new Paragraph(text, GetSmallFont(false)) {Alignment = Element.ALIGN_CENTER});
            c.Colspan = colspan;
            c.HorizontalAlignment = Element.ALIGN_CENTER;
            c.VerticalAlignment = Element.ALIGN_CENTER;

            table.AddCell(c);
        }

        public void AddSpannedCell(PdfPTable table, string text, int colspan, Font font)
        {
            var c = new PdfPCell(new Paragraph(text, font) {Alignment = Element.ALIGN_CENTER});
            c.Colspan = colspan;
            c.HorizontalAlignment = Element.ALIGN_CENTER;
            c.VerticalAlignment = Element.ALIGN_CENTER;

            table.AddCell(c);
        }

        public void AddSpannedCell(PdfPTable table, string text, int colspan, Font font, float height)
        {
            var c = new PdfPCell(new Paragraph(text, font) {Alignment = Element.ALIGN_CENTER});
            c.Colspan = colspan;
            c.HorizontalAlignment = Element.ALIGN_CENTER;
            c.VerticalAlignment = Element.ALIGN_CENTER;
            c.FixedHeight = height;

            table.AddCell(c);
        }

        public void AddSpannedCell(PdfPTable table, Image image, int colspan, Font font, float height)
        {
            var c = new PdfPCell(image);
            c.Colspan = colspan;
            c.HorizontalAlignment = Element.ALIGN_CENTER;
            c.VerticalAlignment = Element.ALIGN_CENTER;
            c.FixedHeight = height;

            table.AddCell(c);
        }

        public void AddSpannedCell(PdfPTable table, string text, int colspan, Font font, float height, int alignment)
        {
            var c = new PdfPCell(new Paragraph(text, font) {Alignment = Element.ALIGN_CENTER});
            c.Colspan = colspan;
            c.HorizontalAlignment = alignment;
            c.VerticalAlignment = Element.ALIGN_CENTER;
            c.FixedHeight = height;

            table.AddCell(c);
        }

        public void AddSpannedCell(PdfPTable table, Paragraph paragraph, int colspan, float height, int alignment)
        {
            var c = new PdfPCell(paragraph);
            c.Colspan = colspan;
            c.HorizontalAlignment = alignment;
            c.VerticalAlignment = Element.ALIGN_CENTER;
            c.FixedHeight = height;

            table.AddCell(c);
        }

        public Font GetSmallerFont()
        {
            return new Font(GetBaseFont(), 8, Font.NORMAL);
        }

        public Font GetXSmallFont(bool bold)
        {
            return new Font(GetBaseFont(), 6, bold ? Font.BOLD : Font.NORMAL);
        }

        public Font GetXXSmallFont()
        {
            return new Font(GetBaseFont(), 5, Font.NORMAL);
        }

        public Font GetSmallFont(bool bold)
        {
            return new Font(GetBaseFont(), 7, bold ? Font.BOLD : Font.NORMAL);
        }

        public Font GetRegularFont(bool bold)
        {
            return new Font(GetBaseFont(), 8, bold ? Font.BOLD : Font.NORMAL);
        }

        public Font GetLargerFont(bool bold)
        {
            return new Font(GetBaseFont(), 10, bold ? Font.BOLD : Font.NORMAL);
        }

        public Font GetLargeFont(bool bold)
        {
            return new Font(GetBaseFont(), 13, bold ? Font.BOLD : Font.NORMAL);
        }

        public Font GetXLargeFont(bool bold)
        {
            return new Font(GetBaseFont(), 15, bold ? Font.BOLD : Font.NORMAL);
        }

        public BaseFont GetBaseFont()
        {
            return BaseFont.CreateFont("c:\\windows\\fonts\\Arial.ttf", BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        }

        public void AddPage()
        {
            Document.NewPage();
        }
    }
}