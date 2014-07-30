using System;
using System.ComponentModel.DataAnnotations.Schema;
using Webcal.DataModel.Properties;

namespace Webcal.DataModel
{
    public class CustomDayOfWeek : BaseNotification
    {
        public int Id { get; set; }

        [NotMapped]
        public bool IsChecked { get; set; }

        public string DayOfWeek { get; set; }

        public static DayOfWeek Parse(string dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case "Monday":
                    return System.DayOfWeek.Monday;

                case "Tuesday":
                    return System.DayOfWeek.Tuesday;

                case "Wednesday":
                    return System.DayOfWeek.Wednesday;

                case "Thursday":
                    return System.DayOfWeek.Thursday;

                case "Friday":
                    return System.DayOfWeek.Friday;

                case "Saturday":
                    return System.DayOfWeek.Saturday;

                case "Sunday":
                    return System.DayOfWeek.Sunday;
            }

            throw new ArgumentException(Resources.ERR_DAY_OF_WEEK_NOT_RECOGNISED);
        }
    }
}
