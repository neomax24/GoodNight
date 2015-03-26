using GoodNightService.Model;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Popups;


namespace GoodNightService
{
    public class GoodNightServiceManager
    {
        private static GoodNightServiceManager ServiceInstance;
        public static GoodNightServiceManager GetInstance()
        {
            if (ServiceInstance == null)
                ServiceInstance = new GoodNightServiceManager();
            return ServiceInstance;
        }

        public GoodNightServiceManager()
        {
            MobileService = new MobileServiceClient(
              "https://remindsleep.azure-mobile.net/",
              "ShNRPgAtPYuLEfhPSZvDtSfKmNSDug97");
            SyncServiceData();
        }

        private async void SyncServiceData()
        {
            if (UserTable == null)
                UserTable = await MobileService.GetTable<Member>().ToListAsync();
            if (UserFriendTable == null)
                UserFriendTable = await MobileService.GetTable<Friend>().ToListAsync();
            CurrentAccount = UserTable[0];
        }
        public async void RegisterAccount(string userName, string password)
        {
            if (userName.Length > 0)
            {
                if (CheckIsExisted(userName) != null)
                {
                    ShowMessage("已经存在此用户了，请直接登录");
                }
                else
                {
                    if (password.Length > 0)
                    {
                        await MobileService.GetTable<Member>().InsertAsync(new Member
                        {
                            Account = userName,
                            Name = "test name",
                            Id = Guid.NewGuid().ToString(),
                            Password = password,
                            Token = ChannelId,
                            ImageUrl = "test url",

                        });
                        ShowMessage("账户注册成功^_^");
                        SyncServiceData();
                    }
                    else
                    {
                        ShowMessage("请输入密码");
                    }
                }
            }
        }


        public void Login(string userName, string password)
        {
            if (userName.Length > 0 && password.Length > 0)
            {
                var user = CheckIsExisted(userName);
                if (user != null && user.Password == password)
                {
                    ShowMessage("登录成功");
                    CurrentAccount = user;
                }
                else if (user != null && user.Password != password)
                {
                    ShowMessage("密码错误");
                    CurrentAccount = null;
                }
                else
                {
                    ShowMessage("账户不存在，请先注册");
                    CurrentAccount = null;
                }
            }
            else
            {
                ShowMessage("请输入正确的账号密码");
                CurrentAccount = null;
            }
        }


        public async void AddFriend( string friendId)
        {
            var member = CheckIsExisted(friendId);
            if (friendId.Length > 0 && member != null)
            {
                var friend = CheckIsFriend(member.Id);
                if (friend != null)
                {
                    await MobileService.GetTable<Friend>().InsertAsync(new Friend
                    {
                        Id = Guid.NewGuid().ToString(),
                        MemberFirst =CurrentAccount.Id,
                        MemberSecond = member.Id
                    });
                    ShowMessage("添加好友成功");
                    UserFriendTable = await MobileService.GetTable<Friend>().ToListAsync();
                }
                else
                {
                    ShowMessage("已经是好友了,请不要反复添加");
                }
            }
            else
            {
                ShowMessage("请输入正确的用户ID");
            }
        }
        public MobileServiceClient MobileService { get; set; }
        public List<Member> UserTable { get; set; }
        public List<Friend> UserFriendTable { get; set; }
        public Member CurrentAccount { get; set; }
        public string ChannelId { get; set; }

        /// <summary>
        /// 检查用户是否存在
        /// </summary>
        /// <returns></returns>
        private Member CheckIsExisted(string account)
        {
            return UserTable.Find(delegate(Member user) { return user.Account == account; });
        }
        /// <summary>
        /// 检查是否已是朋友
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Friend CheckIsFriend(string id)
        {
            Friend friend = UserFriendTable.Find(delegate(Friend fr)
            {
                return (fr.MemberFirst == CurrentAccount.Id && fr.MemberSecond == id) || (fr.MemberSecond == CurrentAccount.Id && fr.MemberFirst == id);
            });
            return friend;
        }
        /// <summary>
        /// 获取好友列表
        /// </summary>
        /// <returns></returns>
        private List<Friend> GetFriendList()
        {
            List<Friend> friends = UserFriendTable.FindAll(delegate(Friend fr)
            {
                return fr.MemberFirst == CurrentAccount.Id || fr.MemberSecond == CurrentAccount.Id;
            });
            return friends;
        }
        private async void ShowMessage(string content)
        {
            MessageDialog messagebox = new MessageDialog(content, "提示");
            await messagebox.ShowAsync();
        }

        public async void PostNotificationAsync(string title,string content)
        {
            try
            {

                await MobileService.InvokeApiAsync("notifyAllUsers",
                    new JObject(new JProperty("toast", title), new JProperty("content", content)));
            }
            catch
            {

            }
        }

    }
}
