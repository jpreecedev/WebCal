namespace TachographReader.DataModel
{
    using System;
    using Shared;
    using Shared.Core;

    public class ThemeSettings : BaseSettings
    {
        public ThemeSettings()
        {
            SelectedTheme = "Silver";
        }

        public string SelectedTheme { get; set; }

        public Uri Source
        {
            get { return new Uri(string.Format("pack://application:,,,/Fluent;component/Themes/Office2010/{0}.xaml", SelectedTheme)); }
        }
    }
}