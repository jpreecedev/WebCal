using Webcal.DataModel;

namespace Webcal.Library.PDF
{
    public interface IPlaque
    {
        void Create(PDFDocument pdfDocument, TachographDocument tachographDocument);
        void CreateFullCertificate(PDFDocument pdfDocument, TachographDocument tachographDocument);
    }
}
