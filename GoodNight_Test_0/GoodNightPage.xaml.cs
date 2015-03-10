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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace GoodNight_Test_0
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class GoodNightPage : Page
    {
        public GoodNightPage()
        {
            var oauthClient = new ClientOAuth();
            this.InitializeComponent();
            uidtest.Text = oauthClient.Uid;
            CreatTable_TimePeriodList();
        }
        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void ListPickerFlyout_ItemsPicked(ListPickerFlyout sender, ItemsPickedEventArgs args)
        {

        }

        private void AppBarToggleButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private List<DB_TimePeriodList> list_timePeriodList;
        private List<DB_TimePointList> list_timePointList;

        private async void get_timePeriodList()
        {
            list_timePeriodList = await getTable_TimePeriodList();
            this.checklist_time_peroid.ItemsSource = list_timePeriodList;
        }
        private async void get_timePointList()
        {
            list_timePointList = await getTable_TimePointList();
            this.time_points_list.ItemsSource = list_timePointList;
        }

        private SQLiteAsyncConnection GetConn()
        {
            return new SQLiteAsyncConnection(ApplicationData.Current.LocalFolder.Path + "\\GoodNight.db");
        }
        private async void CreatTable_TimePeriodList()
        {
            if (await isDataBaseExist()==false)
            {
                SQLiteAsyncConnection conn = GetConn();
                await conn.CreateTableAsync<DB_TimePeriodList>();
                await conn.CreateTableAsync<DB_TimePointList>();
                insert_TimePeriodList(new DB_TimePeriodList("游戏", 30, true));
                insert_TimePeriodList(new DB_TimePeriodList("微博", 45, true));
                insert_TimePointList(new DB_TimePointList("睡觉", new DateTime(2015, 03, 10, 20, 30, 30), true));
                insert_TimePointList(new DB_TimePointList("学习", new DateTime(2015, 03, 10, 20, 30, 30), true));
            }
            get_timePeriodList();
            get_timePointList();
        }
        private async void insert_TimePeriodList(DB_TimePeriodList data)
        {
            SQLiteAsyncConnection conn = GetConn();
            await conn.InsertAsync(data);
        }
        private async void insert_TimePointList(DB_TimePointList data)
        {
            SQLiteAsyncConnection conn = GetConn();
            await conn.InsertAsync(data);
        }
        private static async System.Threading.Tasks.Task<bool> isDataBaseExist()
        {
            string filePath = "GoodNight.db";
            bool isFileExist=true;
            try
            {
                Windows.Storage.StorageFile sf =await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(filePath);
            }
            catch
            {
                isFileExist = false;
            }
            return isFileExist;
        }

        private async Task<List<DB_TimePeriodList>> getTable_TimePeriodList()
        {
            SQLiteAsyncConnection conn = GetConn();
            var query = conn.Table<DB_TimePeriodList>();
            List<DB_TimePeriodList> result = await query.ToListAsync();
            return result;
        }

        private async Task<List<DB_TimePointList>> getTable_TimePointList()
        {
            SQLiteAsyncConnection conn = GetConn();
            var query = conn.Table<DB_TimePointList>();
            List<DB_TimePointList> result = await query.ToListAsync();
            return result;
        }

        private void add_Button_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
