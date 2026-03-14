using System.Text.Json;
using MyBlazorApp.Models;

namespace MyBlazorApp.Services
{
    public class DataService
    {
        private readonly string _usersFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "users.json");
        private readonly string _appointmentsFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "appointments.json");

        private List<User> _users = new();
        private List<Appointment> _appointments = new();
        private int _nextAppointmentId = 1;

        public DataService()
        {
            LoadData();
        }

        private void LoadData()
        {
            if (File.Exists(_usersFile))
            {
                var json = File.ReadAllText(_usersFile);
                _users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            }

            if (File.Exists(_appointmentsFile))
            {
                var json = File.ReadAllText(_appointmentsFile);
                _appointments = JsonSerializer.Deserialize<List<Appointment>>(json) ?? new List<Appointment>();
                _nextAppointmentId = _appointments.Any() ? _appointments.Max(a => a.Id) + 1 : 1;
            }
        }

        private void SaveUsers()
        {
            var json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_usersFile, json);
        }

        private void SaveAppointments()
        {
            var json = JsonSerializer.Serialize(_appointments, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_appointmentsFile, json);
        }

        public bool RegisterUser(User user)
        {
            if (_users.Any(u => u.Email == user.Email))
                return false;

            _users.Add(user);
            SaveUsers();
            return true;
        }

        public User? LoginUser(string email, string password)
        {
            return _users.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        public void AddAppointment(Appointment appointment)
        {
            appointment.Id = _nextAppointmentId++;
            _appointments.Add(appointment);
            SaveAppointments();
        }

        public void DeleteAppointment(int id)
        {
            var appointment = _appointments.FirstOrDefault(a => a.Id == id);
            if (appointment != null)
            {
                _appointments.Remove(appointment);
                SaveAppointments();
            }
        }

        public List<Appointment> GetAppointmentsForUser(string email)
        {
            return _appointments.Where(a => a.UserEmail == email).ToList();
        }

        public List<Appointment> GetAppointments()
        {
            return _appointments;
        }

        public void SaveAppointment(Appointment appointment)
        {
            appointment.Id = _nextAppointmentId++;
            _appointments.Add(appointment);
            SaveAppointments();
        }
    }
}