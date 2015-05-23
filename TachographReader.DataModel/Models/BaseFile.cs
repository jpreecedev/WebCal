namespace TachographReader.DataModel
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using Connect.Shared.Models;

    public class BaseFile : BaseModel
    {
        public string FileName { get; set; }

        [MaxLength]
        public byte[] SerializedFile { get; set; }

        public DateTime Date { get; set; }

        public static byte[] GetStoredFile(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    var buffer = new byte[binaryReader.BaseStream.Length];
                    binaryReader.Read(buffer, 0, buffer.Length);
                    return buffer;
                }
            }
        }
    }
}