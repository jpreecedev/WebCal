namespace Webcal.Shared
{
    public static class Extensions
    {
        public static string DoubleEscape(this string input)
        {
            return input.Replace("\"", "'");
        }
    }
}