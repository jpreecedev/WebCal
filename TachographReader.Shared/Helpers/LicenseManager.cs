namespace TachographReader.Shared.Helpers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Management;

    public static class LicenseManager
    {
        private static string _machineKey;

        public static string GetMachineKey()
        {
            if (!string.IsNullOrEmpty(_machineKey))
            {
                return _machineKey;
            }

            var cpuInfo = string.Empty;
            var moc = new ManagementClass("win32_processor").GetInstances();

            foreach (ManagementObject mo in moc)
            {
                cpuInfo = mo.Properties["processorID"].Value.ToString();
                break;
            }

            var driveLetter = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)).Substring(0, 1);
            var disk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + driveLetter + @":""");
            disk.Get();

            return (_machineKey = GetSubstring(cpuInfo) + GetSubstring(disk["VolumeSerialNumber"].ToString()));
        }

        public static bool IsValid(string serial, out DateTime expirationDate)
        {
            expirationDate = default(DateTime);

            if (string.IsNullOrEmpty(serial))
            {
                return false;
            }

            if (serial.ToCharArray().Any(Char.IsLetter))
            {
                return false;
            }

            string padded = serial.PadRight(18, '0');
            long paddedAsLong;
            if (long.TryParse(padded, out paddedAsLong))
            {
                try
                {
                    expirationDate = new DateTime(paddedAsLong);
                    if (expirationDate.Hour == 0 && expirationDate.Minute == 0 && expirationDate.Second == 0 && expirationDate.Millisecond == 0)
                    {
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public static DateTime? GetExpirationDate(string serial)
        {
            if (string.IsNullOrEmpty(serial))
            {
                return null;
            }

            if (serial.ToCharArray().Any(Char.IsLetter))
            {
                return null;
            }

            string padded = serial.PadRight(18, '0');
            long paddedAsLong;
            if (long.TryParse(padded, out paddedAsLong))
            {
                try
                {
                    var expirationDate = new DateTime(paddedAsLong);
                    if (expirationDate.Hour == 0 && expirationDate.Minute == 0 && expirationDate.Second == 0 && expirationDate.Millisecond == 0)
                    {
                        return expirationDate;
                    }
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        public static bool HasExpired(DateTime expirationDate)
        {
            return expirationDate.Date < DateTime.Now.Date;
        }

        private static string GetSubstring(string key)
        {
            return key.PadRight(2).Substring(0, 2);
        }
    }
}