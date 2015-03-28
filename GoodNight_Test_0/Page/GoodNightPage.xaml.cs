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
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using GoodNightService.Model;

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
            InitializationTimer();
            InitializationDB();
            reflesh_friendList();
        }

        private async void Test_More()
        {

            StorageFolder applicationFolder = ApplicationData.Current.LocalFolder;
            await applicationFolder.CreateFolderAsync("account", CreationCollisionOption.ReplaceExisting);
            StorageFolder imageFolder = await applicationFolder.GetFolderAsync("account");
            await imageFolder.CreateFileAsync("avatar.jpg", CreationCollisionOption.ReplaceExisting);
            StorageFile imageFile = await imageFolder.GetFileAsync("avatar.jpg");
            StorageFile inFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Resource/avatar_test.jpg"));
            await inFile.CopyAndReplaceAsync(imageFile);
            updateAndReflesh_CurrentAccount();
        }


        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        //初始化计时器
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
                s.TimePeriod_barValue = s.get_timePeriod_barValue();
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
            foreach (DB_TimePeriodList s in timePeriodListData)
            {
                if (s.IS_WORK == true)
                {
                    dispatcherTimer.Start();
                    break;
                }
            }
            DB_account_Controll db_account = new DB_account_Controll();
            GoodNightService.Model.Member account_temp=App.GoodNightService.CurrentAccount;

            await db_account.initializate_account(
                new DB_account
                {
                    userID=account_temp.Id,
                    declaration=account_temp.Description,
                    nickName=account_temp.Name,
                    Token=account_temp.Token
                });
            Test_More();
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void ListPickerFlyout_ItemsPicked(ListPickerFlyout sender, ItemsPickedEventArgs args)
        {

        }

        //微博退出登录？？？
        private void logout_test_Button_Click(object sender, RoutedEventArgs e)
        {
            var Weibo_oauthClient = new ClientOAuth();
            Weibo_oauthClient.QuitOAuth();
            Frame frame = new Frame();
            frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(LoginPage));
        }
        
        //晚安自己，添加“早睡计划”按钮响应
        private void timePoint_add_flyout_Click(object sender, RoutedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(timePointList_addPage));
        }
        //添加“帮我计时”按钮相应
        private void timeProid_add_flyout_Click(object sender, RoutedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(timePeriod_addPage));
        }
        //删除“早睡计划”条目
        private async void timePoint_delete_Click(object sender, RoutedEventArgs e)
        {
            DB_TimePointList selectedTimePoint = ((MenuFlyoutItem)sender).DataContext as DB_TimePointList;
            DB_Controller DB = new DB_Controller();
            DB.delete_TimePointList(selectedTimePoint);
            await DB.reflesh_timePoint();
            timePointListData = DB.get_timePointList;
            time_points_list.ItemsSource = timePointListData;
        }

        //删除“帮我计时”条目
        private async void timePeriod_delete_Click(object sender, RoutedEventArgs e)
        {
            DB_TimePeriodList selectedTimePeriod = ((MenuFlyoutItem)sender).DataContext as DB_TimePeriodList;
            DB_Controller DB = new DB_Controller();
            DB.delete_TimePeriodList(selectedTimePeriod);
            await DB.reflesh_timePeriod();
            timePeriodListData = DB.get_timePeriodList;
            time_Period_list.ItemsSource = timePeriodListData;
        }
        //响应点击其余区域，“帮我计时”
        private void timePeriod_stackPanel_Holding(object sender, HoldingRoutedEventArgs e)
        {
            StackPanel stackPanel = sender as StackPanel;
            if (stackPanel != null)
            {
                FlyoutBase.ShowAttachedFlyout(stackPanel);
            }
        }
        //响应点击其余区域，“早睡计划”
        private void timePoint_stackPanel_Holding(object sender, HoldingRoutedEventArgs e)
        {
            StackPanel stackPanel = sender as StackPanel;
            if (stackPanel != null)
            {
                FlyoutBase.ShowAttachedFlyout(stackPanel);
            }
        }
        //“帮我计时”，启动响应
        private async void timePeriod_IsWork_Click(object sender, RoutedEventArgs e)
        {
            DB_TimePeriodList selectedTimePeriod = ((Coding4Fun.Toolkit.Controls.OpacityToggleButton)sender).DataContext as DB_TimePeriodList;
            if (((ToggleButton)sender).IsChecked == true & timePeriodWorkMutexCheck())
            {
                selectedTimePeriod.TIMESTART = DateTime.Now.ToString("s");
                selectedTimePeriod.IS_WORK = true;
                selectedTimePeriod.TimePeriod_barValue = 0;
                selectedTimePeriod.TIMEEND = DateTime.Parse(selectedTimePeriod.TIMESTART).AddMinutes(selectedTimePeriod.TIME_PERIOD).ToString("s");
                foreach(DB_TimePeriodList s in timePeriodListData)
                {
                    if(s.ID==selectedTimePeriod.ID)
                    {
                        timePeriodListData[timePeriodListData.IndexOf(s)] = selectedTimePeriod;
                        break;
                    }
                }
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
            else if (((ToggleButton)sender).IsChecked == false)
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
                ((ToggleButton)sender).IsChecked = false;
                Coding4Fun.Toolkit.Controls.ToastPrompt toast = new Coding4Fun.Toolkit.Controls.ToastPrompt();
                toast.Message = "一心不可二用噢亲";
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
        //"帮我计时"，选择时间 按钮响应
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
        //“早睡计划”，选择时间 按钮响应
        private async void Period_pickerFlyout_TimePicked(TimePickerFlyout sender, TimePickedEventArgs args)
        {
            if (Convert.ToInt32(sender.Time.TotalMinutes) != 0)
            {
                Period_picker_tamp.TIME_PERIOD = Convert.ToInt32(sender.Time.TotalMinutes);
                Period_picker_tamp.TIMESTART = DateTime.Now.ToString("s");
                Period_picker_tamp.TIMEEND = DateTime.Now.AddMinutes(Period_picker_tamp.TIME_PERIOD).ToString("s");
                DB_Controller db = new DB_Controller();
                await db.update_TimePeriodList(Period_picker_tamp);

                foreach (ScheduledToastNotification s in ToastNotificationManager.CreateToastNotifier().GetScheduledToastNotifications())
                {
                    if (s.Id == "Period" + Period_picker_tamp.ID.ToString())
                    {
                        ScheduledToastNotification recurringToast = new ScheduledToastNotification(s.Content, DateTime.Parse(Period_picker_tamp.TIMEEND));
                        ToastNotificationManager.CreateToastNotifier().RemoveFromSchedule(s);
                        ToastNotificationManager.CreateToastNotifier().AddToSchedule(recurringToast);
                        break;
                    }
                }

                foreach (DB_TimePeriodList s in timePeriodListData)
                {
                    if (s.ID == Period_picker_tamp.ID)
                    {
                        s.TIME_PERIOD = Period_picker_tamp.TIME_PERIOD;
                        s.TIMESTART = Period_picker_tamp.TIMESTART;
                        s.TIMEEND = Period_picker_tamp.TIMEEND;
                        Period_picker_tamp = null;
                        break;
                    }
                }
            }
        }
        //“早睡计划”，复选框响应
        private async void timePoint_check_Click(object sender, RoutedEventArgs e)
        { 

            DB_TimePointList selectedTimePoint = ((CheckBox)sender).DataContext as DB_TimePointList;
            if (((CheckBox)sender).IsChecked == true)
            {
                
                selectedTimePoint.IS_WORK = true;
                foreach(DB_TimePointList s in timePointListData)
                {
                    if(s.ID==selectedTimePoint.ID)
                    {
                        s.IS_WORK = selectedTimePoint.IS_WORK;
                        break;
                    }
                }
                DB_Controller db = new DB_Controller();
                await db.update_TimePointList(selectedTimePoint);
                DateTime now = DateTime.Now;
                DateTime notificationTime = new DateTime(now.Year, now.Month, now.Day, (int)selectedTimePoint.TIME_POINT.Hours, (int)selectedTimePoint.TIME_POINT.Minutes, 0);
                if(isTimePassed(selectedTimePoint.TIME_POINT))
                {
                    return;
                }
                XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
                XmlNodeList toastNodeList = toastXml.GetElementsByTagName("text");
                toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode("Time is up"));
                toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode("Time Point toast test"));
                ScheduledToastNotification recurringToast = new ScheduledToastNotification(toastXml, notificationTime);
                recurringToast.Id = "Point" + selectedTimePoint.ID.ToString();
                ToastNotificationManager.CreateToastNotifier().AddToSchedule(recurringToast);
            }
            else
            {
                selectedTimePoint.IS_WORK = false;
                foreach (DB_TimePointList s in timePointListData)
                {
                    if (s.ID == selectedTimePoint.ID)
                    {
                        s.IS_WORK = selectedTimePoint.IS_WORK;
                        break;
                    }
                }
                DB_Controller db = new DB_Controller();
                await db.update_TimePointList(selectedTimePoint);

                foreach (ScheduledToastNotification s in ToastNotificationManager.CreateToastNotifier().GetScheduledToastNotifications())
                {
                    if (s.Id == "Point" + selectedTimePoint.ID.ToString())
                    {
                        ToastNotificationManager.CreateToastNotifier().RemoveFromSchedule(s);
                        break;
                    }
                }
            }
        }
        //“早睡计划”，检查选择的时间是否过期
        private bool isTimePassed(TimeSpan timeSpan)
        {
            DateTime now = DateTime.Now;
            if(timeSpan.Hours<now.Hour)
            {
                return true;
            }
            else if (timeSpan.Hours == now.Hour && timeSpan.Minutes < now.Minute)
            {
                return true;
            }
            return false;
        }
        //晚安自己，话筒按钮响应
        private void Goodnight_Speaker_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(SpeakPage));
        }
        //晚安朋友，添加好友按钮响应
        private void friend_add_button_Click(object sender, RoutedEventArgs e)
        {
            add_friend_controll();
        }
        //晚安朋友，调用api
        private async void add_friend_controll()
        {
            await App.GoodNightService.AddFriend(friend_add_text.Text);
            reflesh_friendList();
        }
        //晚安朋友，更新数据库存储
        private void reflesh_friendList()
        {
            List<Member> friend_list_tamp = new List<Member>();
            foreach (Friend s in App.GoodNightService.UserFriendTable)
            {
                if (s.MemberFirst == App.GoodNightService.CurrentAccount.Id)
                {
                    friend_list_tamp.Add(App.GoodNightService.UserTable.Find(delegate(Member _member) { return _member.Id == s.MemberSecond; }));
                }
                if (s.MemberSecond == App.GoodNightService.CurrentAccount.Id)
                {
                    friend_list_tamp.Add(App.GoodNightService.UserTable.Find(delegate(Member _menber) { return _menber.Id == s.MemberFirst; }));
                }
            }
            Friend_list.ItemsSource = friend_list_tamp;
        }

        //晚安朋友，寻找我的睡友，添加按钮响应
        private void add_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        //更多，“我的宣言”，更改后，取消按钮响应
        private void more_declaration_cancel_Click(object sender, RoutedEventArgs e)
        {
            declaration_flyout.Hide();
        }
        //更多，“我的宣言”，更改后，确认按钮响应
        private void more_declaration_confirm_Click(object sender, RoutedEventArgs e)
        {
            //TODO
            App.GoodNightService.CurrentAccount.Description = more_declaration.Text;
            updateAndReflesh_CurrentAccount();
            declaration_flyout.Hide();
        }
        //更多，更新界面，更新数据库
        private async void updateAndReflesh_CurrentAccount()
        {
            await App.GoodNightService.MobileService.GetTable<Member>().UpdateAsync(App.GoodNightService.CurrentAccount);
            
            StorageFolder applicationFolder = ApplicationData.Current.LocalFolder;
            DB_account_Controll db_account = new DB_account_Controll();
            GoodNightService.Model.Member account_temp = App.GoodNightService.CurrentAccount;

            await db_account.initializate_account(
                new DB_account
                {
                    userID = account_temp.Id,
                    declaration = account_temp.Description,
                    nickName = account_temp.Name,
                    Token = account_temp.Token
                });
            DB_account_Controll moreControll = new DB_account_Controll();
            DB_account more = await moreControll.get_account();
            more_nickName.Text = more.nickName;
            more_nickName_flyout.Text = more.nickName;
            more_sex.SelectedIndex = more.sex;
            declarration_show.Text = more.declaration;
            avatar_img.Source = new BitmapImage(new Uri(applicationFolder.Path + more.avatarPath));
            more_declaration.Text = more.declaration;
        }
        //更多，修改早睡宣言
        private void more_declaration_panel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StackPanel stack = sender as StackPanel;
            FlyoutBase.ShowAttachedFlyout(stack);
        }
        
        //更多，更改昵称
        private void more_name_panel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StackPanel stack = sender as StackPanel;
            FlyoutBase.ShowAttachedFlyout(stack);
        }
        //更多，确认更改昵称
        private void more_nickName_confirm_Click(object sender, RoutedEventArgs e)
        {
            App.GoodNightService.CurrentAccount.Name = more_nickName_flyout.Text;
            updateAndReflesh_CurrentAccount();
            name_flyout.Hide();
        }
        //更多，取消更改昵称
        private void more_nickName_cancel_Click(object sender, RoutedEventArgs e)
        {
            name_flyout.Hide();
        }
        
    }
}
