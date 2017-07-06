namespace TachographReader.Library
{
    using System;
    using System.Linq;

    public static class IMSWarningHelper
    {
        private static string[] AffectedRegistrations = {"62", "13", "63", "14", "64", "15", "65", "16", "66", "17", "67", "18", "68", "19", "69"};

        public static bool IsVehicleAffected(string vehicleRegistration)
        {
            if (!string.IsNullOrWhiteSpace(vehicleRegistration))
            {
                var trimmed = vehicleRegistration.Replace(" ", "").Trim();
                if (trimmed.Length >= 4)
                {
                    var firstTwoCharacters = trimmed.Substring(0, 2);
                    if (firstTwoCharacters.All(c => Char.IsLetter(c)))
                    {
                        var year = trimmed.Substring(2, 2);
                        if (year.All(c => Char.IsNumber(c)))
                        {
                            if (AffectedRegistrations.Any(c => c == year))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}