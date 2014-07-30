using System;
using System.IO;

namespace Webcal.DataModel
{
    public class WorkshopCardFile : BaseFile
    {
        public string Workshop { get; set; }

        public static WorkshopCardFile GetWorkshopCardFile(DateTime dateTime, string workshop, string filePath)
        {
            return new WorkshopCardFile
            {
                Date = dateTime,
                Workshop = workshop,
                FileName = Path.GetFileName(filePath),
                SerializedFile = GetStoredFile(filePath)
            };

        }
    }
}
