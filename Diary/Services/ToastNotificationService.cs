using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Diary.Services {
    static class ToastNotificationService {
        public static void ShowToastImmediately(ToastContent toastContent) {
            ToastNotification toast = new ToastNotification(toastContent.GetXml());

        }

        //private const string TOAST_CONTENT = @"
        //    <toast>

        //    </toast>
        //";

        //public static void ScheduleToast(DateTime scheduledTime) {
        //    XmlDocument xml = new XmlDocument();
        //    xml.LoadXml(TOAST_CONTENT);

        //    ScheduledToastNotification toast = new ScheduledToastNotification(xml, scheduledTime);
        //    toast.Id = "Id" + scheduledTime.ToString();
        //    toast.Tag = "NotificationOne";
        //    toast.Group = "MyEverydayToasts";
        //    ToastNotificationManager.CreateToastNotifier().AddToSchedule(toast);
        //}
    }
}
