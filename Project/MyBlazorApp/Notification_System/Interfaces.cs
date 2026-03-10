namespace Notification_System
{
    public interface IObserver
    {
        void Update(string message);
    }

    public interface ISubject
    {
        void Attach(IObserver o);
        void Notify(string message);
    }
}