using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Utils {
    static class DateUtils {
         public static readonly DateTime UNIXTIME_ZERO_POINT = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static bool IsToday(DateTime date) {
            return date.Date == DateTime.Now.Date;
        }
        public static bool IsYesterday(DateTime date) {
            return IsToday(date.AddDays(1));
        }
        public static bool IsThisYear(DateTime date) {
            return date.Year == DateTime.Now.Year;
        }
        public static string ToDateString(DateTime date) {
            return date.ToString("d.M.yyyy");
        }
        public static bool CompareDay(DateTime d1, DateTime d2) {
            return d1.Date == d2.Date;
        }
        public static long ToUnixtime(DateTime value) {
            return (long) value.Date.ToUniversalTime().Subtract(UNIXTIME_ZERO_POINT).TotalSeconds;
        }
        public static DateTime FromUnixtime(long value) {
            DateTime result = UNIXTIME_ZERO_POINT.AddSeconds(value);
            return result.ToLocalTime().Date;
        }
    }
}
