using System.Collections.Generic;

namespace Notification_System
{
    public class NotificationManager : ISubject
    {
        private readonly List<IObserver> _observers = new List<IObserver>();
        private readonly INotification _notificationMethod;

        // Через конструктор передаємо спосіб сповіщення (Email чи SMS)
        public NotificationManager(INotification notificationMethod)
        {
            _notificationMethod = notificationMethod;
        }

        public void Attach(IObserver o)
        {
            if (!_observers.Contains(o))
            {
                _observers.Add(o);
            }
        }

        public void Notify(string message)
        {
            // 1. Сповіщаємо всі об'єкти (наприклад, клієнтів), які підписалися
            foreach (var observer in _observers)
            {
                observer.Update(message);
            }
            
            // 2. Фізично відправляємо повідомлення
            _notificationMethod.Send(message);
        }
    }
}