using System.Collections.Generic;
using System.Linq;
using Domain_Entities;

namespace Repositories
{
    public class AppointmentRepository : IRepository<Appointment>
    {
        private readonly List<Appointment> _appointments = new List<Appointment>();

        public IEnumerable<Appointment> GetAll() => _appointments;

        public Appointment GetById(int id) => _appointments.FirstOrDefault(a => a.AppointmentId == id);

        public void Save(Appointment entity)
        {
            _appointments.Add(entity);
        }

        public void Delete(int id)
        {
            var appointment = GetById(id);
            if (appointment != null)
            {
                _appointments.Remove(appointment);
            }
        }
    }
}