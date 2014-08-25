namespace Webcal.DataModel.Core
{
    public interface IPrinterSettingsRepository
    {
        PrinterSettings GetSettings();

        void Save(PrinterSettings settings);
    }
}