using System;

namespace LightBlue.MultiHost.Core
{
    public interface INotificationHub
    {
        void Notify(string title, string content, Action onclick);
    }
}