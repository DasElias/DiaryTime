using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace Diary.Services {
    static class DiaryReminderToastService {
        public static void ShowToastImmediately() {
            ToastNotification toast = new ToastNotification(CreateToast().GetXml());
            toast.ExpirationTime = DateTime.Now.Date.AddDays(1);
            toast.Tag = "1822"; // TODO change
            toast.Group = "Hallo";
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public static void ScheduleToast(DateTime d) {
            ScheduledToastNotification toast = new ScheduledToastNotification(CreateToast().GetXml(), new DateTimeOffset(d));
            toast.ExpirationTime = d.Date.AddDays(1);
            toast.Tag = d.ToString();
            toast.Group = "DiaryTime";
            ToastNotificationManager.CreateToastNotifier().AddToSchedule(toast);
        }

        private static ToastContent CreateToast() {
            ToastVisual visual = new ToastVisual() {
                BindingGeneric = new ToastBindingGeneric() {
                    Children = {
                        new AdaptiveText() {
                            Text = "DiaryTime"
                        },
                        new AdaptiveText() {
                            Text = "Du hast heute noch keinen Tagebucheintrag erstellt!"
                        }
                    }
                }
            };
            ToastActionsCustom actions = new ToastActionsCustom() {
                Buttons = {
                    new ToastButton("Jetzt erstellen", new QueryString().ToString()),
                    new ToastButtonDismiss("Ignorieren")
                },

            };
            ToastContent content = new ToastContent() {
                Visual = visual,
                Actions = actions
            };
            content.Scenario = ToastScenario.Reminder;
            return content;
        }
    }
}
