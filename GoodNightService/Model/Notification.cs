using System;

namespace GoodNightService.Model
{
    public class Notification
    {
        public string Id { get; set; }
        public string SendId { get; set; }
        public string ReceiveId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime SendTime { get; set; }
        public bool IsCompleted { get; set; }

    }
}