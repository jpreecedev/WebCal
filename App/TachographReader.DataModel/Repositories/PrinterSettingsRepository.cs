using System.Collections.Generic;
using System.Linq;
using Webcal.DataModel.Core;
using Webcal.DataModel;

namespace Webcal.DataModel.Repositories
{
    public class PrinterSettingsRepository : BaseRepository, IPrinterSettingsRepository
    {
        #region Implementation of IPrinterSettingsRepository

        public PrinterSettings GetSettings()
        {
            List<PrinterSettings> printerSettings = Safely(() => Context.PrinterSettings).ToList();

            if (printerSettings.Count == 0)
            {
                return null;
            }
            if (printerSettings.Count == 1)
            {
                return printerSettings.First();
            }

            return GetPrinterSettings(printerSettings);
        }

        public void Save(PrinterSettings settings)
        {
            Safely(() =>
            {
                Context.PrinterSettings.Attach(settings);
                Context.Entry(settings).State = System.Data.Entity.EntityState.Modified;
                Context.SaveChanges();
            });
        }

        #endregion

        #region Private Methods

        private static PrinterSettings GetPrinterSettings(IEnumerable<PrinterSettings> all)
        {
            return all.FirstOrDefault(w => !string.IsNullOrEmpty(w.DefaultPrinterName));
        }

        #endregion
    }
}
