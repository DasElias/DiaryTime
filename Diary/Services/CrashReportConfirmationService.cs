using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diary.Services {
    static class CrashReportConfirmationService {
        private const string CONFIRMATION_KEY = "crashReportConfirmation";

        public static void Update(bool shouldSend) {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            roamingSettings.Values[CONFIRMATION_KEY] = shouldSend;

            LoadValue();
        }

        public static void LoadValue() {
            UserConfirmation confirmation = ShouldSend() ? UserConfirmation.AlwaysSend : UserConfirmation.DontSend;

            Crashes.NotifyUserConfirmation(confirmation);
        }

        public static bool ShouldSend() {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            object val = roamingSettings.Values[CONFIRMATION_KEY];

            if(val == null) return true;
            else return (bool) val;
        }
    }
}
