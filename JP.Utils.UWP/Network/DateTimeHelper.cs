using System;

namespace JP.Utils.Network
{
    public static class DateTimeHelper
    {
        public static long ToTimestamp(this DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        public static DateTime FromTimestamp(long unixTimestamp)
        {
            DateTime utcDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return utcDateTime.AddSeconds(unixTimestamp).ToLocalTime();
        }
    }
}