using Izeron.Library.Enums;
using System;

namespace Izeron.Library.Notification
{
    public class GameNotification
    {
        public string Body { get; set; }
        public GameNotificationState GameNotificationState { get; set; }
        public bool IsRead { get; set; }
        public string TimeStamp { get; set; } = DateTime.Now.ToShortTimeString();
    }
}
