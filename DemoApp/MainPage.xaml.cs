
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.Storage;
using GoodNightService.Model;
using Windows.Storage;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍

namespace DemoApp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: 准备此处显示的页面。

            // TODO: 如果您的应用程序包含多个页面，请确保
            // 通过注册以下事件来处理硬件“后退”按钮:
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed 事件。
            // 如果使用由某些模板提供的 NavigationHelper，
            // 则系统会为您处理该事件。
        }

        #region 按钮事件
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            App.GoodNightService.RegisterAccount(UserNameTextBox.Text, PasswordTextBox.Password,"test name");
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            App.GoodNightService.Login(UserNameTextBox.Text, PasswordTextBox.Password);
        }
        /// <summary>
        /// 添加好友
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFriendButton_Click(object sender, RoutedEventArgs e)
        {
            App.GoodNightService.AddFriend(FriendIDBox.Text);
        }
        /// <summary>
        /// 发送推送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotificationButton_Click(object sender, RoutedEventArgs e)
        {
            App.GoodNightService.PostNotificationAsync("早点休息，goodnight^_^", "test message");
        }
       
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var file= await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/icon175x175.png"));
            App.GoodNightService.UploadImage(App.GoodNightService.CurrentAccount.Account, file);
        }

        private void AddAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            App.GoodNightService.AddPointment(AppiontmentTimePicker.Time, "提醒记录", "详细信息");

        }
        #endregion
        
       
       




    }
}
