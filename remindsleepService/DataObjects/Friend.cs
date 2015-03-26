using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace remindsleepService.DataObjects
{
    public class Friend:EntityData
    {
        public string SendId { get; set; }
        public string MemberFirst { get; set; }
        public string MemberSecond { get; set; }

    }
}