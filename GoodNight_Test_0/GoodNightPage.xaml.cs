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
        private void InitializationTimer()
        {

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        async void dispatcherTimer_Tick(object sender, object e) 
        {
            DB_Controller DB = new DB_Controller();
            await DB.reflesh_timePeriod();
            
            time_Period_list.ItemsSource = DB.get_timePeriodList;

        }
        private async void InitializationDB()
        {
            DB_Controller DB_myGoodnight = new DB_Controller();
            await DB_myGoodnight.CreatTable();
            this.time_points_list.ItemsSource = DB_myGoodnight.get_timePointList;
            this.time_Period_list.ItemsSource = DB_myGoodnight.get_timePeriodList;
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

        private void timePeriod_ComboBox_DropDownOpened(object sender, object e)
        {
        }

        private async void timePeriod_delete_Click(object sender, RoutedEventArgs e)
        {
            DB_TimePeriodList selectedTimePeriod = ((MenuFlyoutItem)sender).DataContext as DB_TimePeriodList;
            DB_Controller DB = new DB_Controller();
            DB.delete_TimePeriodList(selectedTimePeriod);
            await DB.reflesh_timePeriod();
            time_Period_list.ItemsSource = DB.get_timePeriodList;
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
            time_points_list.ItemsSource = DB.get_timePointList;
        }

        private async void timePeriod_IsWork_Checked(object sender, RoutedEventArgs e)
        {
            DB_TimePeriodList selectedTimePeriod = ((AppBarToggleButton)sender).DataContext as DB_TimePeriodList;
            if (selectedTimePeriod.IS_WORK == false)
            {
                selectedTimePeriod.TIMESTART = DateTime.Now.ToString("s");
                selectedTimePeriod.IS_WORK = true;
                selectedTimePeriod.TIMEEND = DateTime.Parse(selectedTimePeriod.TIMESTART).AddMinutes(selectedTimePeriod.TIME_PERIOD).ToString("s");
                DB_Controller db = new DB_Controller();
                await db.update_TimePeriodList(selectedTimePeriod);
                DB_Controller db_test = new DB_Controller();
                await db_test.reflesh_timePeriod();
                List<DB_TimePeriodList> test_list = db_test.get_timePeriodList;
            }
            //((StackPanel)((StackPanel)((AppBarToggleButton)sender).Parent).Parent).Children.Add(bar);
        }

        private void timePeriod_IsWork_Unchecked(object sender, RoutedEventArgs e)
        {
            DB_TimePeriodList selectedTimePeriod = ((AppBarToggleButton)sender).DataContext as DB_TimePeriodList;
            selectedTimePeriod.IS_WORK = false;
            DB_Controller db = new DB_Controller();
            db.update_TimePeriodList(selectedTimePeriod);
        }


    }
}
