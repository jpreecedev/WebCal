namespace Webcal.Shared
{
    using System;
    using System.Linq;

    public static class LicenseManager
    {
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
    }
}