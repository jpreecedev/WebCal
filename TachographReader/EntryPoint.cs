namespace TachographReader
{
    using System;
    using Microsoft.Shell;

    public class EntryPoint
    {
        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance("TachographReader"))
            {
                var app = new App();
                app.InitializeComponent();
                app.Boot();
                app.Run();
            }
        }
    }
}