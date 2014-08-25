namespace Webcal.Library.PDF
{
    using DataModel;

    public interface IPlaque
    {
        void Create(PDFDocument pdfDocument, TachographDocument tachographDocument);
        void CreateFullCertificate(PDFDocument pdfDocument, TachographDocument tachographDocument);
    }
}