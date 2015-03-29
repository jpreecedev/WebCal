namespace TachographReader.EventArguments
{
    using System;
    using System.Collections.Generic;
    using Connect.Shared.Models;
    using DataModel.Library;
    using Shared;

    public class DriverCardCompletedEventArgs : EventArgs
    {
        public bool AutoRead { get; set; }
        public SmartCardReadOperation Operation { get; set; }
        public CalibrationRecord CalibrationRecord { get; set; }
        public ICollection<CalibrationRecord> CalibrationHistory { get; set; }
        public string DumpFilePath { get; set; }
        public Exception Exception { get; set; }

        public bool IsSuccess
        {
            get { return Exception == null; }
        }
    }
}