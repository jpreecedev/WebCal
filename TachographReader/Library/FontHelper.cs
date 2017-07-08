namespace TachographReader.Library
{
    using System.Collections.Generic;
    using System.Drawing.Text;
    using System.Linq;

    public static class FontHelper
    {
        public static ICollection<string> GetInstalledFonts()
        {
            var installedFontCollection = new InstalledFontCollection();
            return installedFontCollection.Families.Select(c => c.Name).ToList();
        }
    }
}