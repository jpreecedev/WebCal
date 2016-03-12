namespace TachographReader.DataModel
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations.Schema;
    using Shared.Core;

    [Table("PrinterSettings")]
    public class PrinterSettings : BaseSettings
    {
        public bool AlwaysAskForPrinter { get; set; }
        public bool UseDefaultPrinter { get; set; }
        public string DefaultPrinterName { get; set; }
        public int DefaultNumberOfCopies { get; set; }
        public string DefaultLabelPrinter { get; set; }
        public bool IsPortrait { get; set; }
        public bool IsLandscape { get; set; }
        public int LabelNumberOfCopies { get; set; }
        public bool AutoPrintLabels { get; set; }

        [DefaultValue(true)]
        public bool AutoClosePDFProgram{ get; set; }

        [DefaultValue("Lucida Sans Unicode")]
        public string DefaultFont { get; set; }

        [DefaultValue(5)]
        public int Timeout { get; set; }
    }
}