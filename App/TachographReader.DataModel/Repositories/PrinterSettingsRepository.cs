namespace Webcal.DataModel.Repositories
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using Core;

    public class PrinterSettingsRepository : BaseRepository, IPrinterSettingsRepository
    {
        public PrinterSettings GetSettings()
        {
            List<PrinterSettings> printerSettings = Safely(() => Context.PrinterSettings).ToList();

            if (printerSettings.Count == 0)
                return null;
            if (printerSettings.Count == 1)
                return printerSettings.First();

            return GetPrinterSettings(printerSettings);
        }

        public void Save(PrinterSettings settings)
        {
            Safely(() =>
            {
                Context.PrinterSettings.Attach(settings);
                Context.Entry(settings).State = EntityState.Modified;
                Context.SaveChanges();
            });
        }

        private static PrinterSettings GetPrinterSettings(IEnumerable<PrinterSettings> all)
        {
            return all.FirstOrDefault(w => !string.IsNullOrEmpty(w.DefaultPrinterName));
        }
    }
}