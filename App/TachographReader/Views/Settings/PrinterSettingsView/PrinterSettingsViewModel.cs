using System.Collections.ObjectModel;
using System.Linq;
using StructureMap;
using Webcal.Core;
using Webcal.DataModel.Core;
using Webcal.DataModel;
using DefaultPrinterSettings = System.Drawing.Printing.PrinterSettings;

namespace Webcal.Views.Settings
{
    public class PrinterSettingsViewModel : BaseSettingsViewModel
    {
        #region Public Properties

        public ObservableCollection<string> Printers { get; set; }

        public PrinterSettings Settings { get; set; }

        public IPrinterSettingsRepository SettingsRepository { get; set; }

        #endregion

        #region Overrides

        protected override void Load()
        {
            Printers = new ObservableCollection<string>(DefaultPrinterSettings.InstalledPrinters.Cast<string>());
            Printers.Insert(0, string.Empty);

            Settings = SettingsRepository.GetSettings();
            
            if (string.IsNullOrEmpty(Settings.DefaultPrinterName) || !Printers.Contains(Settings.DefaultPrinterName))
                Settings.DefaultPrinterName = Printers.First();

            if (string.IsNullOrEmpty(Settings.DefaultLabelPrinter) || !Printers.Contains(Settings.DefaultLabelPrinter))
                Settings.DefaultLabelPrinter = Printers.First();
        }

        protected override void InitialiseRepositories()
        {
            SettingsRepository = ObjectFactory.GetInstance<IPrinterSettingsRepository>();
        }

        public override void Save()
        {
            SettingsRepository.Save(Settings);
        }

        #endregion
    }
}