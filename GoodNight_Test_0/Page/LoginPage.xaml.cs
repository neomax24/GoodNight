using GoodNight_Test_0.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WeiboSDKForWinRT;
using Windows.ApplicationModel.Activation;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using System.Text.RegularExpressions;
using GoodNightService.Model;
using Windows.UI.Popups;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace GoodNight_Test_0
{
    /// <summary>
    /// 可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public LoginPage()
        {
            this.InitializeComponent();
            InitData();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// 获取与此 <see cref="Page"/> 关联的 <see cref="NavigationHelper"/>。
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// 获取此 <see cref="Page"/> 的视图模型。
        /// 可将其更改为强类型视图模型。
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// 使用在导航过程中传递的内容填充页。  在从以前的会话
        /// 重新创建页时，也会提供任何已保存状态。
        /// </summary>
        /// <param name="sender">
        /// 事件的来源; 通常为 <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">事件数据，其中既提供在最初请求此页时传递给
        /// <see cref="Frame.Navigate(Type, Object)"/> 的导航参数，又提供
        /// 此页在以前会话期间保留的状态的
        /// 字典。 首次访问页面时，该状态将为 null。</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// 保留与此页关联的状态，以防挂起应用程序或
        /// 从导航缓存中放弃此页。值必须符合
        /// <see cref="SuspensionManager.SessionState"/> 的序列化要求。
        /// </summary>
        /// <param name="sender">事件的来源；通常为 <see cref="NavigationHelper"/></param>
        ///<param name="e">提供要使用可序列化状态填充的空字典
        ///的事件数据。</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper 注册

        /// <summary>
        /// 此部分中提供的方法只是用于使
        /// NavigationHelper 可响应页面的导航方法。
        /// <para>
        /// 应将页面特有的逻辑放入用于
        /// <see cref="NavigationHelper.LoadState"/>
        /// 和 <see cref="NavigationHelper.SaveState"/> 的事件处理程序中。
        /// 除了在会话期间保留的页面状态之外
        /// LoadState 方法中还提供导航参数。
        /// </para>
        /// </summary>
        /// <param name="e">提供导航方法数据和
        /// 无法取消导航请求的事件处理程序。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void login_button_Click(object sender, RoutedEventArgs e)
        {
            //Todo
            if (!isEmailLegal(email_textbox.Text))
            {
                ShowMessage("请输入正确的电子邮箱");
                return;
            }
            if (!isPasswordLegal(password_textbox.Password))
            {
                ShowMessage("密码须大于6位");
                return;
            }
            string password_hash = hash(password_textbox.Password);
            App.GoodNightService.Login(email_textbox.Text, password_hash);
            if (App.GoodNightService.CurrentAccount != null)
            {
                Frame frame = Window.Current.Content as Frame;
                frame.Navigate(typeof(GoodNightPage));
            }
        }
        private string hash(string data)
        {
            HashAlgorithmProvider hash = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            Windows.Storage.Streams.IBuffer hash_result = hash.HashData(CryptographicBuffer.ConvertStringToBinary(data, BinaryStringEncoding.Utf8));
            string result = CryptographicBuffer.EncodeToHexString(hash_result);
            return result;
        }
        private bool isEmailLegal(string p)
        {
            string expression = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            if (Regex.IsMatch(p, expression, RegexOptions.None))
            {
                return true;
            }
            return false;
        }

        private bool isPasswordLegal(string p)
        {
            if (p.Length >= 6)
                return true;
            return false;
        }
        private void weibo_login_button_Click(object sender, RoutedEventArgs e)
        {
            Frame frame = new Frame();
            frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(weibo_loginPage));
        }

        private void InitData()
        {
            // TODO:编译运行之前需要开放平台参数.
            SdkData.AppKey = "2321842552";
            SdkData.AppSecret = "48dd08394b50889674c2101e9fc6a0b5";
            SdkData.RedirectUri = "https://api.weibo.com/oauth2/default.html";
            // prepare the pic to be shared.
        }
        private void is_weibo_login()
        {

            var Weibo_oauthClient = new ClientOAuth();
            //测试
            //weibo_uid_test.Text = Weibo_oauthClient.Uid;
            //测试wei
            // 判断是否已经授权或者授权是否过期.
            if (Weibo_oauthClient.IsAuthorized == false)
            {

            }
            else
            {
                Frame frame = new Frame();
                frame = Window.Current.Content as Frame;
                frame.Navigate(typeof(GoodNightPage));
            }
        }

        private void Registor_Click(object sender, RoutedEventArgs e)
        {
            if (!isEmailLegal(email_textbox.Text))
            {
                ShowMessage("请输入正确的电子邮箱");
                return;
            }
            if (!isPasswordLegal(password_textbox.Password))
            {
                ShowMessage("密码须大于6位");
                return;
            }
            App.GoodNightService.RegisterAccount(email_textbox.Text, hash(password_textbox.Password),"Night"+new Random().Next(1000).ToString());
        }
        private async void ShowMessage(string content)
        {
            MessageDialog messagebox = new MessageDialog(content, "提示");
            await messagebox.ShowAsync();
        }
    }
}
