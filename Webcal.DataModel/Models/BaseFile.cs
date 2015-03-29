namespace TachographReader.DataModel
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using Connect.Shared.Models;
    using Shared;
    using Shared.Core;

    public class BaseFile : BaseModel
    {
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