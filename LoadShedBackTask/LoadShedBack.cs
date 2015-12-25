using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel.Background;

namespace Tasks
{
    public sealed class LoadShedBack : IBackgroundTask
    {
        int[] times = {5, 0, 8, 0, 13, 0, 17, 0, 8, 0, 11, 0, 17, 0, 20, 0,
                        10, 0, 14, 0, 19, 30, 21, 30, 6, 0, 9, 0, 14, 0, 18, 0,
                        11, 0, 16, 0, 20, 0, 22, 0, 7, 0, 10, 0, 16, 0, 19, 30, 
                        9, 0, 13, 0, 18, 0, 21, 0};
        int TodayId;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            int offset = 3;
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Object value = localSettings.Values["groupid"];

            if (value == null)
            {
                localSettings.Values["groupid"] = offset;
            }
            else
            {
                offset = (int)value;
            }

            int day = (int)DateTime.Today.DayOfWeek;
            TodayId = day - offset;
            while (TodayId < 0) TodayId = 7 + TodayId;

            value = localSettings.Values["Scheduled"];
            bool ToAddSchedule = false;
            if (value != null)
            {
                ToAddSchedule = (bool)value;
            }

            DeleteSchedule();
            if (ToAddSchedule)
                AddSchedule();
                


            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();
            // asynchronous methods with await keyword here 
            _deferral.Complete();

        }


        private void DeleteSchedule()
        {

            var notifier = Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier();
            var scheduled = notifier.GetScheduledToastNotifications();
            foreach (var notify in scheduled)
                notifier.RemoveFromSchedule(notify);

        }

        void RemainingTime(int index, ref int hr, ref int min, bool togo, int id)
        {
            int sH1 = times[index * 8 + 4 * id];
            int sM1 = times[index * 8 + 1 + 4 * id];
            int eH1 = times[index * 8 + 2 + 4 * id];
            int eM1 = times[index * 8 + 3 + 4 * id];

            if (togo) { hr = sH1; min = sM1; }
            else { hr = eH1; min = eM1; }
        }
        private void AddSchedule()
        {

            var notifier = Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier();
            var scheduled = notifier.GetScheduledToastNotifications();

            var template = Windows.UI.Notifications.ToastTemplateType.ToastText01;
            var toastXml = Windows.UI.Notifications.ToastNotificationManager.GetTemplateContent(template);

            int minuteOffset = 5;
            DateTime[] dts = new DateTime[4 * 2];//;[7 * 4];
            for (int j = 0; j < 2; j++)
            {
                int id = TodayId + j;
                if (id > 6) id = id - 7;
                int hr = 0, min = 0;

                RemainingTime(id, ref hr, ref min, true, 0);
                dts[0 + 4 * j] = DateTime.Today.AddHours(hr).AddMinutes(min - minuteOffset);
                RemainingTime(id, ref hr, ref min, true, 1);
                dts[1 + 4 * j] = DateTime.Today.AddHours(hr).AddMinutes(min - minuteOffset);
                RemainingTime(id, ref hr, ref min, false, 0);
                dts[2 + 4 * j] = DateTime.Today.AddHours(hr).AddMinutes(min - minuteOffset);
                RemainingTime(id, ref hr, ref min, false, 1);
                dts[3 + 4 * j] = DateTime.Today.AddHours(hr).AddMinutes(min - minuteOffset);
            }
            //for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    DateTime d;
                    toastXml.GetElementsByTagName("text")[0].InnerText = "सावधान! लोडसेडिङ्ग हुन लागेको छ ।";

                    d = dts[4 * j + 0].AddDays(j);
                    if (d > DateTime.Now)
                        notifier.AddToSchedule(new Windows.UI.Notifications.ScheduledToastNotification(
                                                toastXml, new DateTimeOffset(d)));
                    d = dts[4 * j + 1].AddDays(j);
                    if (d > DateTime.Now)
                        notifier.AddToSchedule(new Windows.UI.Notifications.ScheduledToastNotification(
                                                toastXml, new DateTimeOffset(d)));

                    toastXml.GetElementsByTagName("text")[0].InnerText = "बधार्इ छ! लोडसेडिङ्ग सकिन लागेको छ ।";

                    d = dts[4 * j + 2].AddDays(j);
                    if (d > DateTime.Now)
                        notifier.AddToSchedule(new Windows.UI.Notifications.ScheduledToastNotification(
                                                toastXml, new DateTimeOffset(d)));
                    d = dts[4 * j + 3].AddDays(j);
                    if (d > DateTime.Now)
                        notifier.AddToSchedule(new Windows.UI.Notifications.ScheduledToastNotification(
                                                toastXml, new DateTimeOffset(d)));
                }
            }

            // Windows.Storage.ApplicationData.Current.LocalSettings.Values["schedDate"] = DateTime.Today.Ticks;

        }
    }
}
