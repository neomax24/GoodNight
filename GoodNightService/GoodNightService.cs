using GoodNightService.Model;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Popups;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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

		#region 属性
		public MobileServiceClient MobileService { get; set; }
		public ObservableCollection<Member> UserTable { get; set; }
		public ObservableCollection<Friend> UserFriendTable { get; set; }
		public Member CurrentAccount { get; set; }
		public string ChannelId { get; set; }
		#endregion



		private async void SyncServiceData()
        {
			if (UserTable == null)
				UserTable = await MobileService.GetSyncTable<Member>().ToCollectionAsync();
			if (UserFriendTable == null)
				UserFriendTable = await MobileService.GetSyncTable<Friend>().ToCollectionAsync();
            CurrentAccount = UserTable[0];
        }


		#region API方法
		/// <summary>
		/// 注册
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns>返回int值，提示内容请看注释</returns>
		public async Task<int> RegisterAccount(string userName, string password)
        {
            if (userName.Length > 0)
            {
                if (CheckIsExisted(userName) != null)
                {
					//ShowMessage("已经存在此用户了，请直接登录");
					return 1;
                }
                else
                {
                    if (password.Length > 0)
                    {
                        await MobileService.GetSyncTable<Member>().InsertAsync(new Member
                        {
                            Account = userName,
                            Name = "test name",
                            Id = Guid.NewGuid().ToString(),
                            Password = password,
                            Token = ChannelId,
                            ImageUrl = "test url",

                        });
						// ShowMessage("账户注册成功^_^");
						return 0;
                    }
                    else
                    {
						//ShowMessage("请输入密码");
						return 2;
                    }
                }
            }
			return 2;
        }

		/// <summary>
		/// 登陆
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns>返回int值，提示内容请看注释</returns>
		public int Login(string userName, string password)
        {
            if (userName.Length > 0 && password.Length > 0)
            {
                var user = CheckIsExisted(userName);
                if (user != null && user.Password == password)
                {
                   // ShowMessage("登录成功");
                    CurrentAccount = user;
					return 0;
                }
                else if (user != null && user.Password != password)
                {
					//ShowMessage("密码错误");
					return 1;
                }
                else
                {
					// ShowMessage("账户不存在，请先注册");
					return 2;
                }
            }
            else
            {
				// ShowMessage("请输入正确的账号密码");
				return 1;
            }
        }

		/// <summary>
		/// 添加朋友
		/// </summary>
		/// <param name="friendId"></param>
		/// <returns>返回int值，提示内容请看注释</returns>
        public async Task<int> AddFriend( string friendId)
        {
            var member = CheckIsExisted(friendId);
            if (friendId.Length > 0 && member != null)
            {
                var friend = CheckIsFriend(member.Id);
                if (friend != null)
                {
                    await MobileService.GetSyncTable<Friend>().InsertAsync(new Friend
                    {
                        Id = Guid.NewGuid().ToString(),
                        MemberFirst =CurrentAccount.Id,
                        MemberSecond = member.Id
                    });
					
                    //ShowMessage("添加好友成功");
					UserFriendTable = await MobileService.GetSyncTable<Friend>().ToCollectionAsync();
					return 0;
                }
                else
                {
					//ShowMessage("已经是好友了,请不要反复添加");
					return 1;
                }
            }
            else
            {
				// ShowMessage("请输入正确的用户ID");
				return 2;
            }

        }
		
        

        /// <summary>
        /// 检查用户是否存在
        /// </summary>
        /// <returns></returns>
        private Member CheckIsExisted(string account)
        {
            return UserTable.FirstOrDefault<Member>(delegate(Member user) { return user.Account == account; });
        }
        /// <summary>
        /// 检查是否已是朋友
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Friend CheckIsFriend(string id)
        {
            Friend friend = UserFriendTable.FirstOrDefault<Friend>(delegate(Friend fr)
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
            var friends = UserFriendTable.Where(delegate(Friend fr)
            {
                return fr.MemberFirst == CurrentAccount.Id || fr.MemberSecond == CurrentAccount.Id;
            });
            return friends.ToList<Friend>();
        }
        private async void ShowMessage(string content)
        {
            MessageDialog messagebox = new MessageDialog(content, "提示");
            await messagebox.ShowAsync();
        }
		/// <summary>
		/// 推送给指定的人
		/// </summary>
		/// <param name="title"></param>
		/// <param name="content"></param>
		/// <param name="target">指定用户id</param>
        public async void PostNotificationAsync(string title,string content,string target)
        {
            try
            {

                await MobileService.InvokeApiAsync("notifyAllUsers",
                    new JObject(new JProperty("toast", title), new JProperty("content", content),new JProperty("target",target)));
            }
            catch
            {

            }
        }
		/// <summary>
		/// 推送给所有人
		/// </summary>
		/// <param name="title"></param>
		/// <param name="content"></param>
		public async void PostNotificationAsync(string title, string content)
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
		#endregion
	}
}
