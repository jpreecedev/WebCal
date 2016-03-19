namespace TachographReader.Library.PDF
{
    using System.IO;
    using Connect.Shared.Models;

    public class PDFDocumentResult
    {
        public byte[] SerializedData
        {
            get
            {
                if (Document != null)
                {
                    return Document.SerializedData ?? LoadFileData();
                }
                if (Report != null)
                {
                    return Report.SerializedData ?? LoadFileData();
                }
                return LoadFileData();
            }
        }

        public Document Document { get; set; }

        public BaseReport Report { get; set; }

        public GV212Report GV212Report { get; set; }

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

        private byte[] LoadFileData()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                return null;
            }

            return File.ReadAllBytes(FilePath);
        }
    }
}