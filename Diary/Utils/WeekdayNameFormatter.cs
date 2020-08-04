using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace Diary.Utils {
    class WeekdayNameFormatter {
        public WeekdayNameFormatter(ResourceLoader resourceLoader) {
            Monday = resourceLoader.GetString("abbrMonday");
            Tuesday = resourceLoader.GetString("abbrTuesday");
            Wednesday = resourceLoader.GetString("abbrWednesday");
            Thursday = resourceLoader.GetString("abbrThursday");
            Friday = resourceLoader.GetString("abbrFriday");
            Saturday = resourceLoader.GetString("abbrSaturday");
            Sunday = resourceLoader.GetString("abbrSunday");


        }

        public string Monday { get; }
        public string Tuesday { get; }
        public string Wednesday { get; }
        public string Thursday { get; }
        public string Friday { get; }
        public string Saturday { get; }
        public string Sunday { get; }
    }
}
