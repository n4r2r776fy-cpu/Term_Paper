using Domain_Entities;
using Repositories;
using Notification_System;

namespace Services
{
    public class AuthService
    {
        public void Login(Person p) { /* Логіка входу */ }
        public void Logout(Person p) { /* Логіка виходу */ }
    }

    public class AppointmentService
    {
        private readonly IRepository<Appointment> _repo;
        private readonly ISubject _notificationManager;

        public AppointmentService(IRepository<Appointment> repo, ISubject notificationManager)
        {
            _repo = repo;
            _notificationManager = notificationManager;
        }

        public void CreateAppointment(Appointment data)
        {
            _repo.Save(data);
            _notificationManager.Notify($"Створено новий запис на {data.Date.ToShortDateString()}");
        }

        public void CancelAppointment(int id)
        {
            _repo.Delete(id);
            _notificationManager.Notify($"Запис скасовано");
        }
    }
}