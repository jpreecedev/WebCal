namespace Webcal.Shared
{
    using System.IO;

    public static class Extensions
    {
        public static string DoubleEscape(this string input)
        {
            return input.Replace("\"", "'");
        }

        public static void EmptyFolder(this DirectoryInfo directoryInfo)
        {
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo subfolder in directoryInfo.GetDirectories())
            {
                EmptyFolder(subfolder);
            }
        }
    }
}