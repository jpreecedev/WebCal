namespace TachographReader.Library.PDF
{
    using System.IO;
    using Connect.Shared.Models;

    public class PDFDocumentResult
    {
        public Document Document { get; set; }

        public string FilePath { get; set; }

        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(FilePath))
                {
                    return string.Empty;
                }

                return Path.GetFileNameWithoutExtension(FilePath);
            }
        }

        public bool Success
        {
            get { return !string.IsNullOrEmpty(FilePath); }
        }
    }
}