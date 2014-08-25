namespace Webcal.DataModel
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PrinterSettings")]
    public class PrinterSettings : BaseModel
    {
        public int Id { get; set; }

        public bool AlwaysAskForPrinter { get; set; }

        public bool UseDefaultPrinter { get; set; }

        public string DefaultPrinterName { get; set; }

        public string DefaultLabelPrinter { get; set; }

        public bool IsPortrait { get; set; }

        public bool IsLandscape { get; set; }
    }
}