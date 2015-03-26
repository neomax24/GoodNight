using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace remindsleepService.DataObjects
{
    public class Member : EntityData
    {
        
  
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///登录账号
        /// 邮箱或者手机号码
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 用户票据
        /// Guid类型
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 用户头像图片地址
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 用户自我描述
        /// </summary>
        public string Description { get; set; }
        public string Password { get; set; }

    }
}