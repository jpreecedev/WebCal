namespace Webcal.DataModel
{
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
    }
}