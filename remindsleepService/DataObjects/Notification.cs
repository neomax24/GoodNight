using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace remindsleepService.DataObjects
{
    public class Notification:EntityData
    {
        public string SendId { get; set; }
        public string ReceiveId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime SendTime { get; set; }
        public bool IsCompleted { get; set; }

    }
}