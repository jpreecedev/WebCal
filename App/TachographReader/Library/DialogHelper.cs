namespace Webcal.Library
{
    using System;
    using Microsoft.Win32;

    public static class DialogHelper
    {
        public static DialogHelperResult OpenFile(DialogFilter filter, string fileName)
        {
            var dialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Multiselect = false,
                FileName = fileName,
                Filter = GetActualFilter(filter)
            };

            bool? result = dialog.ShowDialog();
            return new DialogHelperResult {Result = result, FileName = dialog.FileName};
        }

        public static DialogHelperResult SaveFile(DialogFilter filter, string fileName)
        {
            var dialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = fileName,
                Filter = GetActualFilter(filter)
            };

            bool? result = dialog.ShowDialog();
            return new DialogHelperResult {Result = result, FileName = dialog.FileName};
        }

        private static string GetActualFilter(DialogFilter filter)
        {
            switch (filter)
            {
                case DialogFilter.SQLServerCEDatabaseFile:
                    return "SQL Server Compact Edition Database File (*.sdf)|*.sdf";

                case DialogFilter.PDF:
                    return "Portable Document Format (PDF) (*.pdf)|*.pdf";

                case DialogFilter.PlainText:
                    return "Text File (*.txt)|*.txt";

                case DialogFilter.Codecs:
                    return GetCodecFilter();

                case DialogFilter.Data:
                    return "Data File (*.dat)|*.dat";

                case DialogFilter.JPG:
                    return "JPEG File (*.jpg)|*.jpg";

                case DialogFilter.Empty:
                    return "";

                default:
                    return "All Files (*.*)|*.*";
            }
        }

        private static string GetCodecFilter()
        {
            return "Portable Network Graphics (*.png)|*.png|All Files (*.*)|*.*";
        }
    }

    public enum DialogFilter
    {
        SQLServerCEDatabaseFile,
        PDF,
        PlainText,
        Codecs,
        Data,
        All,
        JPG,
        Empty
    }
}