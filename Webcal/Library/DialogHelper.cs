using Webcal.Properties;

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
                    return Resources.TXT_SQL_SERVER_COMPACT_EDITION;

                case DialogFilter.PDF:
                    return Resources.TXT_PORTABLE_DOCUMENT_FORMAT;

                case DialogFilter.PlainText:
                    return Resources.TXT_TEXT_FILE;

                case DialogFilter.Codecs:
                    return GetCodecFilter();

                case DialogFilter.Data:
                    return Resources.TXT_DATA_FILE;

                case DialogFilter.JPG:
                    return Resources.TXT_JPEG_FILE;

                case DialogFilter.Empty:
                    return string.Empty;

                default:
                    return Resources.TXT_ALL_FILES;
            }
        }

        private static string GetCodecFilter()
        {
            return Resources.TXT_PORTABLE_NETWORK_GRAPHICS;
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