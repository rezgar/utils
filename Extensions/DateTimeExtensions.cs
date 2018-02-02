using System;
using System.Collections.Generic;
using System.Text;

namespace Rezgar.Utils.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToUtc(this DateTime dateTime, TimeZoneInfo sourceTimeZone = null)
        {
            return dateTime.ToTimeZone(TimeZoneInfo.Utc, sourceTimeZone);
        }
        public static DateTime ToTimeZone(this DateTime dateTime, TimeZoneInfo timeZone, TimeZoneInfo sourceTimeZone = null)
        {
            return TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone ?? TimeZoneInfo.Local, timeZone);
        }
    }
}
