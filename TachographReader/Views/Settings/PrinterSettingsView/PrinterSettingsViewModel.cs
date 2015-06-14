namespace TachographReader.Views.Settings
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing.Text;
    using System.Linq;
    using Core;
    using DataModel;
    using DataModel.Core;
    using DataModel.Library;
    using Shared;
    using DefaultPrinterSettings = System.Drawing.Printing.PrinterSettings;

    public class PrinterSettingsViewModel : BaseSettingsViewModel
    {
        public ObservableCollection<string> Printers { get; set; }
        public ObservableCollection<string> Fonts { get; set; }
        public PrinterSettings Settings { get; set; }
        public ISettingsRepository<PrinterSettings> SettingsRepository { get; set; }

        protected override void Load()
        {
            Printers = new ObservableCollection<string>(DefaultPrinterSettings.InstalledPrinters.Cast<string>());
            Printers.Insert(0, string.Empty);

            Settings = SettingsRepository.GetPrinterSettings();

            if (string.IsNullOrEmpty(Settings.DefaultPrinterName) || !Printers.Contains(Settings.DefaultPrinterName))
            {
                Settings.DefaultPrinterName = Printers.First();
            }

            if (string.IsNullOrEmpty(Settings.DefaultLabelPrinter) || !Printers.Contains(Settings.DefaultLabelPrinter))
            {
                Settings.DefaultLabelPrinter = Printers.First();
            }

            Fonts = new ObservableCollection<string>(GetInstalledFonts());
        }

        protected override void InitialiseRepositories()
        {
            SettingsRepository = GetInstance<ISettingsRepository<PrinterSettings>>();
        }

        public override void Save()
        {
            SettingsRepository.Save(Settings);
        }

        private static ICollection<string> GetInstalledFonts()
        {
            InstalledFontCollection installedFontCollection = new InstalledFontCollection();
            return installedFontCollection.Families.Select(c => c.Name).ToList();
        }
    }
}