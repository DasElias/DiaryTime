using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Services {
    static class SpellCheckingSettingService {
        private const string KEY = "SpellChecking";

        public static void SetSpellCheckingPreference(bool isEnabled) {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values[KEY] = isEnabled;
        }

        public static bool GetSpellCheckingPreference() {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            object value = roamingSettings.Values[KEY];
            if(value == null) {
                return false;
            } else {
                return (bool) value;
            }
        }
    }
}
