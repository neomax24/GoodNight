using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using WeiboSDKForWinRT;
using SQLite;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;
using Coding4Fun;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace GoodNight_Test_0
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class GoodNightPage : Page
    {

        private List<DB_TimePeriodList> timePeriodListData=new List<DB_TimePeriodList>();
        private List<DB_TimePointList> timePointListData=new List<DB_TimePointList>();

        public GoodNightPage()
        {
            var oauthClient = new ClientOAuth();
            this.InitializeComponent();
            uidtest.Text = oauthClient.Uid;
            Initialization();
        }
        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        private void Initialization()
        {
            InitializationDB();
            InitializationTimer();
        }
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private void InitializationTimer()
        {

            
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
        }

        async void dispatcherTimer_Tick(object sender, object e) 
        {
            DB_Controller DB = new DB_Controller();
            await DB.reflesh_timePeriod();
            foreach(DB_TimePeriodList s in timePeriodListData)
            {
                s.TimePeriod_barValue = timePeriodListData[timePeriodListData.IndexOf(s)].get_timePeriod_barValue();
            }

        }
        private async void InitializationDB()
        {
            DB_Controller DB_myGoodnight = new DB_Controller();
            await DB_myGoodnight.CreatTable();
            timePeriodListData = DB_myGoodnight.get_timePeriodList;
            timePointListData = DB_myGoodnight.get_timePointList;
            this.time_points_list.ItemsSource = timePointListData;
            time_Period_list.ItemsSource = timePeriodListData;
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void ListPickerFlyout_ItemsPicked(ListPickerFlyout sender, ItemsPickedEventArgs args)
        {

        }


        private void logout_test_Button_Click(object sender, RoutedEventArgs e)
        {
            var Weibo_oauthClient = new ClientOAuth();
            Weibo_oauthClient.QuitOAuth();
            Frame frame = new Frame();
            frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(LoginPage));
        }

        private void add_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void timePoint_add_flyout_Click(object sender, RoutedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(timePointList_addPage));
        }

        private void timeProid_add_flyout_Click(object sender, RoutedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(timePeriod_addPage));
        }

        private async void timePeriod_delete_Click(object sender, RoutedEventArgs e)
        {
            DB_TimePeriodList selectedTimePeriod = ((MenuFlyoutItem)sender).DataContext as DB_TimePeriodList;
            DB_Controller DB = new DB_Controller();
            DB.delete_TimePeriodList(selectedTimePeriod);
            await DB.reflesh_timePeriod();
            timePeriodListData = DB.get_timePeriodList;
            time_Period_list.ItemsSource = timePeriodListData;
        }

        private void timePeriod_stackPanel_Holding(object sender, HoldingRoutedEventArgs e)
        {
            StackPanel stackPanel = sender as StackPanel;
            if (stackPanel != null)
            {
                FlyoutBase.ShowAttachedFlyout(stackPanel);
            }
        }

        private void timePoint_stackPanel_Holding(object sender, HoldingRoutedEventArgs e)
        {
            StackPanel stackPanel = sender as StackPanel;
            if (stackPanel != null)
            {
                FlyoutBase.ShowAttachedFlyout(stackPanel);
            }
        }

        private async void timePoint_delete_Click(object sender, RoutedEventArgs e)
        {
            DB_TimePointList selectedTimePoint = ((MenuFlyoutItem)sender).DataContext as DB_TimePointList;
            DB_Controller DB = new DB_Controller();
            DB.delete_TimePointList(selectedTimePoint);
            await DB.reflesh_timePoint();
            timePointListData = DB.get_timePointList;
            time_points_list.ItemsSource = timePointListData;
        }
        private async void timePeriod_IsWork_Click(object sender, RoutedEventArgs e)
        {
            DB_TimePeriodList selectedTimePeriod = ((Coding4Fun.Toolkit.Controls.OpacityToggleButton)sender).DataContext as DB_TimePeriodList;
            if (((Coding4Fun.Toolkit.Controls.OpacityToggleButton)sender).IsChecked == true & timePeriodWorkMutexCheck())
            {
                selectedTimePeriod.TIMESTART = DateTime.Now.ToString("s");
                selectedTimePeriod.IS_WORK = true;
                selectedTimePeriod.TimePeriod_barValue = 0;
                selectedTimePeriod.TIMEEND = DateTime.Parse(selectedTimePeriod.TIMESTART).AddMinutes(selectedTimePeriod.TIME_PERIOD).ToString("s");
                DB_Controller db = new DB_Controller();
                await db.update_TimePeriodList(selectedTimePeriod);
                
                XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                XmlNodeList toastNodeList = toastXml.GetElementsByTagName("text");
                toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode("Time is up"));
                toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode("Time Period toast test"));
                ScheduledToastNotification recurringToast = new ScheduledToastNotification(toastXml, DateTime.Parse(selectedTimePeriod.TIMEEND));
                recurringToast.Id = "Period"+selectedTimePeriod.ID.ToString();
                ToastNotificationManager.CreateToastNotifier().AddToSchedule(recurringToast);

                dispatcherTimer.Start();
            }
            else if (((Coding4Fun.Toolkit.Controls.OpacityToggleButton)sender).IsChecked == false)
            {
                selectedTimePeriod.IS_WORK = false;
                DB_Controller db = new DB_Controller();
                await db.update_TimePeriodList(selectedTimePeriod);

                DB_Controller db_test = new DB_Controller();
                await db_test.reflesh_timePeriod();
                List<DB_TimePeriodList> test_list = db_test.get_timePeriodList;
                dispatcherTimer.Stop();
                foreach (ScheduledToastNotification s in ToastNotificationManager.CreateToastNotifier().GetScheduledToastNotifications())
                {
                    if(s.Id=="Period"+selectedTimePeriod.ID.ToString())
                    {
                        ToastNotificationManager.CreateToastNotifier().RemoveFromSchedule(s);
                        break;
                    }
                }
            }
            else
            {
                ((Coding4Fun.Toolkit.Controls.OpacityToggleButton)sender).IsChecked = false;
                Coding4Fun.Toolkit.Controls.ToastPrompt toast = new Coding4Fun.Toolkit.Controls.ToastPrompt();
                toast.Message = "一心不可二用poi";
                toast.Show();
            }
        }

        private bool timePeriodWorkMutexCheck()
        {
            DB_Controller db = new DB_Controller();
            
            foreach (DB_TimePeriodList s in timePeriodListData)
            {
                if (s.IS_WORK == true)
                    return false;
            }
            return true;
        }

        private DB_TimePeriodList Period_picker_tamp = new DB_TimePeriodList();
        private void Period_picker_button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                ((TimePickerFlyout)FlyoutBase.GetAttachedFlyout(button)).Time = TimeSpan.FromMinutes(Convert.ToDouble(button.Content.ToString()));
                Period_picker_tamp = button.DataContext as DB_TimePeriodList;
                FlyoutBase.ShowAttachedFlyout(button);
            }
        }

        private async void Period_pickerFlyout_TimePicked(TimePickerFlyout sender, TimePickedEventArgs args)
        {
            if (Convert.ToInt32(sender.Time.TotalMinutes) != 0)
            {
                Period_picker_tamp.TIME_PERIOD = Convert.ToInt32(sender.Time.TotalMinutes);
                DB_Controller db = new DB_Controller();
                await db.update_TimePeriodList(Period_picker_tamp);
                foreach (DB_TimePeriodList s in timePeriodListData)
                {
                    if (s.ID == Period_picker_tamp.ID)
                    {
                        s.TIME_PERIOD = Period_picker_tamp.TIME_PERIOD;
                        Period_picker_tamp = null;
                        break;
                    }
                }
            }
        }

    }
}
