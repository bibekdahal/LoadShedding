using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.ApplicationModel.Background;
using Windows.UI.ApplicationSettings;

using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace LoadShedding
{
    /*struct lTime
    {
        int startH, startM;
        int endH, endM;
    };*/
    public sealed partial class MainPage : Page
    {

        DispatcherTimer timer;
        string[] weeks={"आईतबार", "सोमबार", "मंगलबार", "बुधबार", "बिहीबार", "शुक्रबार", "शनिबार"};
       /* int[] times = {5, 0, 8, 0, 13, 0, 17, 0, 8, 0, 11, 0, 17, 0, 20, 0,
                        10, 0, 14, 0, 19, 30, 21, 30, 6, 0, 9, 0, 14, 0, 18, 0,
                        11, 0, 16, 0, 20, 0, 22, 0, 7, 0, 10, 0, 16, 0, 19, 30, 
                        9, 0, 13, 0, 18, 0, 21, 0};*/

        int[] times = {3,0,9,0,12,0,18,0,10,0,17,0,20,0,1,0,9,0,16,0,19,0,24,0,7,0,14,0,
                        18,0,23,0,6,0,13,0,17,0,22,0,5,0,12,0,16,0,21,0,4,0,10,0,14,0,20,0};

        /*string[] times = { "०५:०० - ०८:००\n १३:०० - १७:००", "०८:०० - ११:००\n १८:०० - २०:००", 
                         "१०:०० - १४:००\n १९:३० - २१:३०", "०९:०० - १३:००\n १९:०० - २१:००",
                         "११:०० - १५:००\n २०:०० - २२:००", "०६:३० - १०:००\n १७:०० - १९:३०",
                         "०६:०० - ०९:००\n १५:०० - १८:००"};*/

        bool TwelveHours=false;
        string getNepVal(int num)
        {
            string str = "";
            while (num != 0)
            {
                int a = (num % 10);
                str = (char)((int)'०' + a) + str;

                num /= 10;
            }

            while (str.Length < 2) str = '०' + str;
            ///else if (str.Length == 0) str = "००";
            return str;
        }
        
        string getNepTime(int index)
        {
            string str;
            int sH1 = times[index * 8];
            int sM1 = times[index * 8 + 1];
            int eH1 = times[index * 8 + 2];
            int eM1 = times[index * 8 + 3];

            int sH2 = times[index * 8 + 4];
            int sM2 = times[index * 8 + 5];
            int eH2 = times[index * 8 + 6];
            int eM2 = times[index * 8 + 7];

            if (!TwelveHours)
                str = getNepVal(sH1) + ":" + getNepVal(sM1) + " - " + getNepVal(eH1) + ":" + getNepVal(eM1) + "\n" +
                     getNepVal(sH2) + ":" + getNepVal(sM2) + " - " + getNepVal(eH2) + ":" + getNepVal(eM2);
            else
            {
                /*bool s1, e1, s2, e2;
                s1 = e1 = s2 = e2 = false;
                if (sH1 > 12) { sH1 -= 12; s1 = true; }
                if (eH1 > 12) { eH1 -= 12; e1 = true; }
                if (sH2 > 12) { sH2 -= 12; s2 = true; }
                if (eH2 > 12) { eH2 -= 12; e2 = true; }

                str = getNepVal(sH1) + ":" + getNepVal(sM1) + (s1 ? " pm" : " am") +
                    " - " + getNepVal(eH1) + ":" + getNepVal(eM1) + (e1 ? " pm" : " am") +
                     "\n" + getNepVal(sH2) + ":" + getNepVal(sM2) + (s2 ? " pm" : " am") +
                     " - " + getNepVal(eH2) + ":" + getNepVal(eM2) + (e2 ? " pm" : " am");*/
                str = getNepVal(sH1) + ":" + getNepVal(sM1) + //(s1 ? " दि." : " बि.") +
                    " - " + getNepVal(eH1) + ":" + getNepVal(eM1) + //(e1 ? " दि." : " बि.") +
                     "\n" + getNepVal(sH2) + ":" + getNepVal(sM2) + //(s2 ? " दि." : " बि.") +
                     " - " + getNepVal(eH2) + ":" + getNepVal(eM2);// +(e2 ? " दि." : " बि.");
            }

            return str;
        }

        void UpdateInfo(int index)
        {
            int sH1 = times[index * 8];
            int sM1 = times[index * 8 + 1];
            int eH1 = times[index * 8 + 2];
            int eM1 = times[index * 8 + 3];

            int sH2 = times[index * 8 + 4];
            int sM2 = times[index * 8 + 5];
            int eH2 = times[index * 8 + 6];
            int eM2 = times[index * 8 + 7];

            int nId = index + 1;
            if (nId > 6) nId = 0;

            int sH3 = times[nId * 8];
            int sM3 = times[nId * 8 + 1];

            int cH = DateTime.Now.Hour;
            int cM = DateTime.Now.Minute;
            
            int id=4;
            if (cH < sH1) id = 0;
            else if (cH == sH1 && cM < sM1) id = 0;
            else if (cH < eH1) id = 1;
            else if (cH == eH1 && cM < eM1) id = 1;
            else if (cH < sH2) id = 2;
            else if (cH == sH2 && cM < sM2) id = 2;
            else if (cH < eH2) id = 3;
            else if (cH == eH2 && cM < eM2) id = 3;

            int ct = cH * 60 + cM;
            int nt;
            if (id == 0) nt = sH1 * 60 + sM1;
            else if (id == 1) nt = eH1 * 60 + eM1;
            else if (id == 2) nt = sH2 * 60 + sM2;
            else if (id == 3) nt = eH2 * 60 + eM2;
            else nt = sH3 * 60 + sM3 + (24 * 60 - ct);

            int rt = (id <= 3) ? nt - ct : nt;
            int rH = rt / 60;
            int rM = rt % 60;

            string str = "लोडसेडिङ्ग ";
            if (id % 2 == 0) str += "हुन ";
            else str += "सकिन ";
            str += "बाँकी समयः\n";
            if (rH != 0) str += getNepVal(rH) + " घण्टा "; 
            str += getNepVal(rM) + " मिनेट";

            InfoBlock.Text = str;           
        }

        void ChangeTable(int groupIndex)
        {
            
            TextBlock[] texts = { Text1, Text2, Text3, Text4, Text5, Text6, Text7 };
            string[] data = new string[7];
            for (int i = 0; i < 7; i++)
            {
                data[i] = weeks[i];
                data[i] += "\n";

                int j = i - groupIndex;
                if (j < 0) j = 7 + j;
                data[i] += getNepTime(j);//times[j];
            }

            int day = (int)DateTime.Today.DayOfWeek;
            for (int i = day-4; i < day+3; i++)
            {
                int j;
                if (i < 0) j = 7 + i;
                else if (i > 6) j = i - 7;
                else j = i;

                texts[i - (day - 4)].Text = data[j];
            }

            NextButton.Visibility = (groupIndex < 6) ? Visibility.Visible : Visibility.Collapsed;
            PrevButton.Visibility = (groupIndex > 0) ? Visibility.Visible : Visibility.Collapsed;

        }

        private async void RegisterBackTask()
        {
            
            var taskRegistered = false;
            var taskName = "LoadShedBack";

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    taskRegistered = true;
                    break;
                }
            }
            if (!taskRegistered)
            {
                TimeTrigger dailyTrigger = new TimeTrigger(60 * 24, false);
                await BackgroundExecutionManager.RequestAccessAsync();

                var builder = new BackgroundTaskBuilder();

                builder.Name = taskName;
                builder.TaskEntryPoint = "Tasks.LoadShedBack";
                builder.SetTrigger(dailyTrigger);
                builder.AddCondition(new SystemCondition(SystemConditionType.UserPresent));

                BackgroundTaskRegistration task = builder.Register();

            }
        }

        int defGroup;
        public MainPage()
        {
            this.InitializeComponent();

            SettingsPane.GetForCurrentView().CommandsRequested += CommandsRequested;

            InfoBlock.Text = "";


            timer = new DispatcherTimer();
            timer.Tick += timer_tick;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

            int group = 4;
            int offset = group - 1;

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Object value = localSettings.Values["groupid"];

            if (value == null)
            {
                localSettings.Values["groupid"] = offset;
            }
            else
            {
                offset = (int)value;
                group = offset + 1;
            }
            defGroup = offset;

            value = localSettings.Values["Scheduled"];
            bool ToAddSchedule = false;
            if (value == null)
            {
                ToAddSchedule = true;
                value = true;
                localSettings.Values["Scheduled"] = value;
            }
           
            CheckNote.IsChecked = (bool)value;


            string samuha = "समूह-";
            for (int i = 0; i < 7; i++)
            {
                char ch = (char)(i+1 + (int)'०');
                GroupBox.Items.Add(samuha + ch);
            }
            GroupBox.SelectedIndex = offset;
            
            //ChangeTable(offset);
            

            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

            if (ToAddSchedule)
                AddSchedule();

            RegisterBackTask();          

        }

        int TodayId;
        void timer_tick(object sender, object e)
        {
            UpdateInfo(TodayId);
        }

       
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void GroupBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (defGroup == GroupBox.SelectedIndex)
            {
                DefButton.Content = "Default Group";
                DefButton.IsEnabled = false;
            }
            else
            {
                DefButton.Content = "Set Default Group";
                DefButton.IsEnabled = true;
            }

            int day = (int)DateTime.Today.DayOfWeek;
            TodayId = day - GroupBox.SelectedIndex;
            while (TodayId < 0) TodayId = 7 + TodayId;

            ChangeTable(GroupBox.SelectedIndex);

            UpdateInfo(TodayId);
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            GroupBox.SelectedIndex++;
            //ChangeTable(GroupBox.SelectedIndex);
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            GroupBox.SelectedIndex--;
            //ChangeTable(GroupBox.SelectedIndex);
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {   
            if (e.VirtualKey == Windows.System.VirtualKey.Left)
            { if (GroupBox.SelectedIndex > 0) GroupBox.SelectedIndex--; }
            else if (e.VirtualKey == Windows.System.VirtualKey.Right)
            { if (GroupBox.SelectedIndex < 6) GroupBox.SelectedIndex++; }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["groupid"] = GroupBox.SelectedIndex;
            defGroup = GroupBox.SelectedIndex;
            DefButton.Content = "Default Group";
            DefButton.IsEnabled = false;

            if (CheckNote.IsChecked == true)
            {
                DeleteSchedule();
                AddSchedule();
            }
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


        private void CheckNote_Click(object sender, RoutedEventArgs e)
        {
            if (CheckNote.IsChecked == true)
            {
                DeleteSchedule();
                AddSchedule();
            }
            else
                DeleteSchedule();

            Windows.Storage.ApplicationData.Current.LocalSettings.Values["Scheduled"] = CheckNote.IsChecked;

        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string str = "";
            var notifier = Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier();
            var scheduled = notifier.GetScheduledToastNotifications();
            foreach (var notify in scheduled)
                str += notify.DeliveryTime.ToString() + "\n";

            MessageDialog msg = new MessageDialog(str);
            await msg.ShowAsync();
        }

        private void CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            SettingsCommand cmd = new SettingsCommand("about_settings", "About", async (x) =>
            {
                string str;
                str = "Loadshedding Schedule App for Windows 8 \nVersion 1.0 \nCopyright © 2013 Bibek Dahal\n\n";
                MessageDialog msg = new MessageDialog(str, "About");
                await msg.ShowAsync();
            });

            args.Request.ApplicationCommands.Add(cmd);
        }

    }
}
