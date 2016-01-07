using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace TachographReader.Library
{
    public static class RegistryHelper
    {
        public static void RegisterProtocol()
        {
            var subkey = Registry.ClassesRoot.CreateSubKey("wcconnect");
            subkey.SetValue("", "URL:Webcal Connect Protocol");
            subkey.SetValue("URL Protocol", "");

            var shell = subkey.CreateSubKey("shell");
            shell.SetValue("", "open");
            var open = shell.CreateSubKey("open");
            open.SetValue("", "");
            var command = open.CreateSubKey("command");

            command.SetValue("", "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"" + " " + "\"" + "%1" + "\"" + " " + "\"" + "%2" + "\"" + " " + "\"" + "%3" + "\"");
        }
    }
}
