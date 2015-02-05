namespace Webcal.DataModel
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Connect.Shared.Models;
    using Properties;
    using Shared;
    using Shared.Core;

    public class CustomDayOfWeek : BaseNotification
    {
        public int Id { get; set; }

        [NotMapped]
        public bool IsChecked { get; set; }

        public string DayOfWeek { get; set; }

        public static DayOfWeek Parse(string dayOfWeek)
        {
            if (dayOfWeek == Resources.TXT_MONDAY)
            {
                return System.DayOfWeek.Monday;
            }
            if (dayOfWeek == Resources.TXT_TUESDAY)
            {
                return System.DayOfWeek.Tuesday;
            }
            if (dayOfWeek == Resources.TXT_WEDNESDAY)
            {
                return System.DayOfWeek.Wednesday;
            }
            if (dayOfWeek == Resources.TXT_THURSDAY)
            {
                return System.DayOfWeek.Thursday;
            }
            if (dayOfWeek == Resources.TXT_FRIDAY)
            {
                return System.DayOfWeek.Friday;
            }
            if (dayOfWeek == Resources.TXT_SATURDAY)
            {
                return System.DayOfWeek.Saturday;
            }
            if (dayOfWeek == Resources.TXT_SUNDAY)
            {
                return System.DayOfWeek.Sunday;
            }

            throw new ArgumentException(Resources.ERR_DAY_OF_WEEK_NOT_RECOGNISED);
        }
    }
}