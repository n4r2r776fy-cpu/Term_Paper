using System;

namespace Notification_System
{
    public interface INotification
    {
        void Send(string message);
    }

    public class EmailNotification : INotification
    {
        public void Send(string message)
        {
            Console.WriteLine($"[Email відправлено]: {message}");
        }
    }

    public class SmsNotification : INotification
    {
        public void Send(string message)
        {
            Console.WriteLine($"[SMS відправлено]: {message}");
        }
    }
}