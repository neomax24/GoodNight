﻿using System;
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
using Windows.Storage.Pickers;
using Windows.Web.Http;
using Microsoft.WindowsAzure;

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

        FileOpenPicker imgpicker = new FileOpenPicker();


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
            InitializationImgPicker();
            InitializationTimer();
            InitializationDB();
        }

        private void InitializationImgPicker()
        {
            imgpicker.FileTypeFilter.Add(".jpg");
            imgpicker.FileTypeFilter.Add(".png");
            imgpicker.ContinuationData["Operation"] = "Image";
        }

        private async Task Test_More()
        {
            await updateAndReflesh_CurrentAccount();
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
            await Test_More();
            await reflesh_friendList();
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
                toast.Message = "一心不可二用——教练";
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


        private void more_declaration_cancel_Click(object sender, RoutedEventArgs e)
        {
            declaration_flyout.Hide();
        }

        private void more_declaration_confirm_Click(object sender, RoutedEventArgs e)
        {
            //TODO
            App.GoodNightService.CurrentAccount.Description = more_declaration.Text;
            updateAndReflesh_CurrentAccount();
            declaration_flyout.Hide();
        }

        private async Task updateAndReflesh_CurrentAccount()
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
                    Token = account_temp.Token,
                    avatarPath = "avatar_" + App.GoodNightService.CurrentAccount.Id +"_"+App.GoodNightService.CurrentAccount.ImageUrl.GetHashCode()+".png",
                });
            DB_account_Controll moreControll = new DB_account_Controll();
            DB_account more = await moreControll.get_account();
            more_nickName.Text = more.nickName;
            more_nickName_flyout.Text = more.nickName;
            more_sex.SelectedIndex = more.sex;
            declarration_show.Text = more.declaration;

            StorageFile avatarFile = await DownloadImgToFile(await applicationFolder.CreateFolderAsync("account", CreationCollisionOption.OpenIfExists), more.avatarPath, App.GoodNightService.CurrentAccount.ImageUrl);
            BitmapImage bitmap_avatar = new BitmapImage();
            bitmap_avatar.SetSource(await avatarFile.OpenAsync(FileAccessMode.Read));
            avatar_img.Source = bitmap_avatar;
            more_declaration.Text = more.declaration;
        }
        private async Task<StorageFile> DownloadImgToFile(StorageFolder Folder,string fileName,string uri)
        {
            bool isExists = false;
            try
            {
                StorageFile file = await Folder.CreateFileAsync(fileName, CreationCollisionOption.FailIfExists);
            }
            catch
            {
                isExists = true;
            }
            if (isExists != true)
            {
                Stream stream = await UriToStream(new Uri(uri));
                StorageFile file = await Folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                using (IRandomAccessStream output = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    Stream outputstream = WindowsRuntimeStreamExtensions.AsStreamForWrite(output.GetOutputStreamAt(0));
                    await stream.CopyToAsync(outputstream);
                    outputstream.Dispose();
                    stream.Dispose();
                }
                return file;
            }
            else
            {
                return await Folder.GetFileAsync(fileName);
            }
        }

        private async Task<Stream> UriToStream(Uri uri)
        {
            var rass = RandomAccessStreamReference.CreateFromUri(uri);
            IRandomAccessStream inputStream = await rass.OpenReadAsync();
            Stream input = WindowsRuntimeStreamExtensions.AsStreamForRead(inputStream.GetInputStreamAt(0));
            return input;
        }


        private async Task<bool> isAvatarExist(string avatarPath)
        {
            try
            {
                StorageFile avatar = await StorageFile.GetFileFromApplicationUriAsync(new Uri(ApplicationData.Current.LocalFolder.Path + avatarPath));
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void more_declaration_panel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StackPanel stack = sender as StackPanel;
            FlyoutBase.ShowAttachedFlyout(stack);
        }

        private void friend_add_button_Click(object sender, RoutedEventArgs e)
        {
            add_friend_controll();
        }
        private async void add_friend_controll()
        {
            await App.GoodNightService.AddFriend(friend_add_text.Text);
            reflesh_friendList();
        }

        private async Task reflesh_friendList()
        {
            List<Member> friend_listAsMember = new List<Member>();
            foreach (Friend s in App.GoodNightService.UserFriendTable)
            {
                if (s.MemberFirst == App.GoodNightService.CurrentAccount.Id)
                {
                    friend_listAsMember.Add(App.GoodNightService.UserTable.Find(delegate(Member _member) { return _member.Id == s.MemberSecond; }));
                }
                if (s.MemberSecond == App.GoodNightService.CurrentAccount.Id)
                {
                    friend_listAsMember.Add(App.GoodNightService.UserTable.Find(delegate(Member _member) { return _member.Id == s.MemberFirst; }));
                }
            }
            List<DB_Friend> friendList = new List<DB_Friend>();
            StorageFolder friendFolder=  await ApplicationData.Current.LocalFolder.CreateFolderAsync("Friends",CreationCollisionOption.OpenIfExists);
            foreach(Member s in friend_listAsMember)
            {
                friendList.Add(new DB_Friend
                {
                    userID = s.Id,
                    NAME = s.Name,
                    IMAGE_PATH =ApplicationData.Current.LocalFolder.Path+ "//Friends//avatar_" + s.Id + "_" + s.ImageUrl.GetHashCode() + ".png",
                });
                if (s.ImageUrl != "test url")
                {
                    await DownloadImgToFile(friendFolder, "avatar_" + s.Id + "_" + s.ImageUrl.GetHashCode() + ".png", s.ImageUrl);
                }
            }
            Friend_list.ItemsSource = friendList;
        }

        private void more_name_panel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StackPanel stack = sender as StackPanel;
            FlyoutBase.ShowAttachedFlyout(stack);
        }

        private void more_nickName_confirm_Click(object sender, RoutedEventArgs e)
        {
            App.GoodNightService.CurrentAccount.Name = more_nickName_flyout.Text;
            updateAndReflesh_CurrentAccount();
            name_flyout.Hide();
        }

        private void more_nickName_cancel_Click(object sender, RoutedEventArgs e)
        {
            name_flyout.Hide();
        }

        private void avatar_img_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image img=sender as Image;
            avatar_img_detail.Source = avatar_img.Source;
            FlyoutBase.ShowAttachedFlyout(img);
        }

        private async void avatar_change_Click(object sender, RoutedEventArgs e)
        {
            StorageFile img_FilePicked = await imgpicker.PickSingleFileAsync();
            DB_account db_account=new DB_account();
            if(img_FilePicked!=null)
            {
                StorageFolder accountFolder = await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalFolder.Path);
                accountFolder = await accountFolder.CreateFolderAsync("account", CreationCollisionOption.OpenIfExists);
                StorageFile avatarFile = await img_FilePicked.CopyAsync(accountFolder, "avatar_" + db_account.userID, NameCollisionOption.ReplaceExisting);
                App.GoodNightService.UploadImage(App.GoodNightService.CurrentAccount.Account, avatarFile);
            }
            avatar_flyout.Hide();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            avatar_flyout.Hide();
        }
    }
}
