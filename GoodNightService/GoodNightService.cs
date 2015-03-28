using GoodNightService.Model;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Popups;
using Windows.ApplicationModel.Appointments;
using Windows.Foundation;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;

namespace GoodNightService
{
    public class GoodNightServiceManager
    {
        private static GoodNightServiceManager ServiceInstance;
        /// <summary>
        /// 获取GoodNight服务实例
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 同步云端数据
        /// </summary>
        private async void SyncServiceData()
        {
            if (UserTable == null)
                UserTable = await MobileService.GetTable<Member>().ToListAsync();
            if (UserFriendTable == null)
                UserFriendTable = await MobileService.GetTable<Friend>().ToListAsync();
            CurrentAccount = UserTable[0];
            storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=goodnightuest;AccountKey=ZQ8gE8awneR7IcJ9g/oVyLx/+j3BQPKShpKiSRBmdfzdBZO6nx4UzAIw8HNK5NmJKQHyq64gz1H5f/qROOh1UQ==;BlobEndpoint=https://goodnightuest.blob.core.windows.net/");
            var blobClient = storageAccount.CreateCloudBlobClient();
            Container = blobClient.GetContainerReference("image");
        }
        /// <summary>
        /// 账户注册
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        public async void RegisterAccount(string userName, string password,string name)
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
                            Name = name,
                            Id = Guid.NewGuid().ToString(),
                            Password = password,
                            Token = ChannelId,
                            //默认头像
                            ImageUrl = "https://goodnightuest.blob.core.windows.net/image/edf1a7a9-8e07-48f1-841a-7df0afc86c54.png",

                        });
                        
                        ShowMessage("账户注册成功^_^");
                        UserTable = await MobileService.GetTable<Member>().ToListAsync();
                    }
                    else
                    {
                        ShowMessage("请输入密码");
                    }
                }
            }
        }

        /// <summary>
        /// 账户登录
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
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
        /// 添加朋友
        /// </summary>
        /// <param name="friendId">朋友ID</param>
        public async Task AddFriend( string friendId)
        {
            UserFriendTable = await MobileService.GetTable<Friend>().ToListAsync();
          //  UserTable = await MobileService.GetTable<Member>().ToListAsync();
            var member = CheckIsExisted(friendId);
            if (friendId.Length > 0 && member != null)
            {
                Friend friend = CheckIsFriend(member.Id);
                if (friend == null)
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
        public CloudStorageAccount storageAccount { get; set; }
        public CloudBlobContainer Container { get; set; }
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
        /// <param name="id">好友ID</param>
        /// <returns></returns>
        private Friend CheckIsFriend(string id)
        {
            Friend friend = null;
            friend = UserFriendTable.Find(delegate(Friend fr)
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
        /// <summary>
        /// 发送推送
        /// </summary>
        /// <param name="title">推送标题</param>
        /// <param name="content">推送内容</param>
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
        /// <summary>
        /// 上传用户头像
        /// </summary>
        /// <param name="id">用户ID</param>
        public async void UploadUserImage(string id,string path)
        {
            var member = CheckIsExisted(id);
            if (member != null)
            {
                
                //MemoryStream ms=imageSource.

               
                await MobileService.GetTable<Member>().UpdateAsync(member);
            }
        }
        public async void UploadImage(string id, StorageFile image)
        {
            var member = CheckIsExisted(id);
            if (member != null)
            {
                //var file=StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///"))
                var name=Guid.NewGuid().ToString()+".png";
                UploadFileToContainer(Container, image, name);
                //MemoryStream ms=imageSource.
                member.ImageUrl = "https://goodnightuest.blob.core.windows.net/image/" + name;

                await MobileService.GetTable<Member>().UpdateAsync(member);
            }
        }
        /// <summary>
        /// 添加提醒事件
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="detail">详细信息</param>
        /// <param name="subject">主题</param>
        public async void AddPointment(TimeSpan time,string detail,string subject)
        {
            var appointment = new Appointment { 
                
                Details=detail,
                Subject=subject,
                StartTime=DateTime.Now
            };
           
            await Windows.ApplicationModel.Appointments.AppointmentManager.ShowAddAppointmentAsync(appointment, new Rect(0,0,480,480), Windows.UI.Popups.Placement.Default);


        }

        private async void UploadFileToContainer(CloudBlobContainer container, StorageFile file, string uplodedFileName)
        {
            var blockBlob = container.GetBlockBlobReference(uplodedFileName);

            await blockBlob.UploadFromFileAsync(file);
           
        }


    }
}
