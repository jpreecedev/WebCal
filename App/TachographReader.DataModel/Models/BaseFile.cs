using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Webcal.DataModel
{
    public class BaseFile : BaseModel
    {
        public int Id { get; set; }

        public string FileName { get; set; }

        [MaxLength]
        public byte[] SerializedFile { get; set; }

        public DateTime Date { get; set; }

        public static byte[] GetStoredFile(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }
    }
}
