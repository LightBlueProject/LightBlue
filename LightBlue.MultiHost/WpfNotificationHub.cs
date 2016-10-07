using System;
using LightBlue.MultiHost.Core;

namespace LightBlue.MultiHost
{
    public class WpfNotificationHub : INotificationHub
    {
        public void Notify(string title, string content, Action onclick)
        {
            NotificationHub.Notify(title, content, onclick);
        }
    }
}