using DemoApp.Model;
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
        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserNameTextBox.Text.Length > 0)
            {
                if (CheckIsExisted(UserNameTextBox.Text) != null)
                {
                    ShowMessage("已经存在此用户了，请直接登录");
                }
                else
                {
                    if (PasswordTextBox.Password.Length > 0)
                    {
                        await App.MobileService.GetTable<Member>().InsertAsync(new Member
                        {
                            Account = UserNameTextBox.Text,
                            Name = "test name",
                            Id = Guid.NewGuid().ToString(),
                            Password = PasswordTextBox.Password,
                            Token = App.ChannelId,
                            ImageUrl = "test url",
                            
                        });
                        ShowMessage("账户注册成功^_^");
                    }
                    else
                    {
                        ShowMessage("请输入密码");
                    }
                }
            }
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserNameTextBox.Text.Length > 0 && PasswordTextBox.Password.Length > 0)
            {
                var user = CheckIsExisted(UserNameTextBox.Text);
                if (user != null && user.Password == PasswordTextBox.Password)
                {
                    ShowMessage("登录成功");
                    App.CurrentAccount = user;
                }
                else if (user != null && user.Password != PasswordTextBox.Password)
                {
                    ShowMessage("密码错误");
                }
                else
                {
                    ShowMessage("账户不存在，请先注册");
                }
            }
            else
            {
                ShowMessage("请输入正确的账号密码");
            }
        }
        /// <summary>
        /// 添加好友
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFriendButton_Click(object sender, RoutedEventArgs e)
        {
            if (FriendIDBox.Text.Length > 0 && CheckIsExisted(FriendIDBox.Text) != null)
            {

            }
        }
        #endregion
        /// <summary>
        /// 检查用户是否存在
        /// </summary>
        /// <returns></returns>
        private Member CheckIsExisted(string account)
        {
            return App.UserTable.Find(delegate(Member user) { return user.Account == account; });
        }
        private async void ShowMessage(string content)
        {
            MessageDialog messagebox = new MessageDialog(content, "提示");
            await messagebox.ShowAsync();
        }

        private async void NotificationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await App.remindsleepClient.InvokeApiAsync("notifyAllUsers",
                    new JObject(new JProperty("toast", "早点休息，goodnight^_^")));
            }
            catch
            {

            }
        }




    }
}
