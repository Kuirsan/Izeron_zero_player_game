using Izeron.Library.Enums;
using System;

namespace Izeron.Library.Notification
{
    public class GameNotification
    {
        public string body;
        public GameNotificationState gameNotificationState;
        public bool isRead;
        public string TimeStamp = DateTime.Now.ToShortTimeString();
    }
}
