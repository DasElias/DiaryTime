using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Services {
    static class ReminderTimeSaveService {
        private static readonly TimeSpan DEFAULT_REMINDER_TIME = new TimeSpan(19, 0, 0);
        private const string REMINDER_TIME_KEY = "ReminderTime";
        private const string SHOULD_REMINDER_KEY = "ShouldReminder";


        public static void SetReminderTime(bool shouldReminder, TimeSpan reminderTime) {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values[REMINDER_TIME_KEY] = reminderTime;
            roamingSettings.Values[SHOULD_REMINDER_KEY] = shouldReminder;
        }

        public static TimeSpan? GetReminderTimeOrNone() {
            if(ShouldReminder()) return GetReminderTime();
            else return null;

        }

        public static bool ShouldReminder() {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            object value = roamingSettings.Values[SHOULD_REMINDER_KEY];
            if(value == null) {
                return false;
            } else {
                return (bool) value;
            }
        }

        public static TimeSpan GetReminderTime() {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            object value = roamingSettings.Values[REMINDER_TIME_KEY];
            if(value == null) {
                return DEFAULT_REMINDER_TIME;
            } else {
                return (TimeSpan) value;
            }
        }

    }
}
