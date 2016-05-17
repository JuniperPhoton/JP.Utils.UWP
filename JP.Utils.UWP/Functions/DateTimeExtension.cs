using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.Utils.Functions
{
    public static class DateTimeExtension
    {
        public static string GetDateTimeIn24Format(this DateTime time)
        {
            return time.ToString("yyyy/MM/dd HH:mm");
        }

        public static string GetDateTimeIn24FormatWithSec(this DateTime time)
        {
            return time.ToString("yyyy/MM/dd HH:mm:ss");
        }

        public static string GetDateTimeIn12Format(this DateTime time)
        {
            return time.ToString("yyyy/MM/dd hh:mm");
        }

        public static string GetDateTimeIn12FormatWithSec(this DateTime time)
        {
            return time.ToString("yyyy/MM/dd hh:mm:ss");
        }
    }
}
