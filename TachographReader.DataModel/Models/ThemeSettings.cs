namespace TachographReader.DataModel
{
    using System;
    using Connect.Shared;
    
    public class ThemeSettings : BaseSettings
    {
        public ThemeSettings()
        {
            SelectedTheme = "Silver";
        }

        public string SelectedTheme { get; set; }

        public Uri Source
        {
            get { return new Uri($"pack://application:,,,/Fluent;component/Themes/Office2010/{SelectedTheme}.xaml"); }
        }
    }
}