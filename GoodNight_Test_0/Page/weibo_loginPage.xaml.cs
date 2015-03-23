using System;
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
using WeiboSDKForWinRT;
using RestSharp;
using Windows.ApplicationModel.Activation;
using System.Threading.Tasks;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace GoodNight_Test_0
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class weibo_loginPage : Page, IWebAuthenticationContinuable
    {

        public weibo_loginPage()
        {
            this.InitializeComponent();
            weibo_login();
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void weibo_login()
        {
            await Task.Delay(1000);
            var Weibo_oauthClient = new ClientOAuth();
            Weibo_oauthClient.BeginOAuth();
        }

        public void ContinueWebAuthentication(WebAuthenticationBrokerContinuationEventArgs ex)
        {
            var oauthClient = new ClientOAuth();
            oauthClient.LoginCallback += (isSucces, err, response) =>
            {
                if (isSucces)
                {
                    // TODO: deal the OAuth result.
                    Frame frame = new Frame();
                    frame = Window.Current.Content as Frame;
                    frame.Navigate(typeof(GoodNightPage));
                }
                else
                {
                    // TODO: handle the err.
                    Frame frame = new Frame();
                    frame = Window.Current.Content as Frame;
                    frame.Navigate(typeof(LoginPage));
                }
            };
            oauthClient.continueAuth(ex.WebAuthenticationResult);
        }
    }
}
