namespace TachographReader
{
    using System;
    using System.Net;
    using Microsoft.Shell;

    public class EntryPoint
    {
        [STAThread]
        public static void Main()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

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