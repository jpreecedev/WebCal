namespace TachographReader.Library.PDF
{
    using Connect.Shared.Models;
    using DataModel;

    public interface IPlaque
    {
        void Create(PDFDocument pdfDocument, TachographDocument tachographDocument);
        void CreateFullCertificate(PDFDocument pdfDocument, TachographDocument tachographDocument, bool excludeLogos);
    }
}