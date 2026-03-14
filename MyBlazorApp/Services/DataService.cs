using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using MyBlazorApp.Models;

namespace MyBlazorApp.Services
{
    public class DataService
    {
        private readonly string _appointmentsFilePath;
        private readonly string _usersFilePath;
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        public DataService(IWebHostEnvironment environment)
        {
            var dataDirectory = Path.Combine(environment.ContentRootPath, "App_Data");
            Directory.CreateDirectory(dataDirectory);

            _appointmentsFilePath = Path.Combine(dataDirectory, "appointments.json");
            _usersFilePath = Path.Combine(dataDirectory, "users.json");

            EnsureSeedData();
        }

        public IReadOnlyList<Doctor> GetDoctors() =>
            new List<Doctor>
            {
                new() { Name = "Erica Lim", Specialty = "Терапевт", AvatarPath = "/lib/icons/avatar1.svg" },
                new() { Name = "Kevin Zane", Specialty = "Стоматолог", AvatarPath = "/lib/icons/avatar2.svg" },
                new() { Name = "Mina Park", Specialty = "Дерматолог", AvatarPath = "/lib/icons/avatar1.svg" }
            };

        public List<Appointment> GetAppointments()
        {
            var appointments = LoadList<Appointment>(_appointmentsFilePath);
            return appointments
                .OrderBy(a => a.Date)
                .ThenBy(a => ParseTime(a.Time))
                .ThenBy(a => a.Name)
                .ToList();
        }

        public void SaveAppointment(Appointment appointment)
        {
            var appointments = GetAppointments();
            appointment.Id = appointments.Any() ? appointments.Max(a => a.Id) + 1 : 1;
            appointment.CreatedAt = DateTime.Now;
            appointment.Status = string.IsNullOrWhiteSpace(appointment.Status) ? AppointmentStatuses.Upcoming : appointment.Status;
            appointment.Price = ResolvePrice(appointment.Service, appointment.Price);
            appointments.Add(appointment);
            SaveList(_appointmentsFilePath, appointments);
        }

        public void UpdateAppointment(Appointment appointment)
        {
            var appointments = GetAppointments();
            var index = appointments.FindIndex(a => a.Id == appointment.Id);
            if (index < 0)
            {
                return;
            }

            appointment.Price = ResolvePrice(appointment.Service, appointment.Price);
            appointments[index] = appointment;
            SaveList(_appointmentsFilePath, appointments);
        }

        public void UpdateAppointmentStatus(int id, string status)
        {
            var appointment = GetAppointments().FirstOrDefault(a => a.Id == id);
            if (appointment is null)
            {
                return;
            }

            appointment.Status = status;
            UpdateAppointment(appointment);
        }

        public void TogglePaid(int id)
        {
            var appointment = GetAppointments().FirstOrDefault(a => a.Id == id);
            if (appointment is null)
            {
                return;
            }

            appointment.Paid = !appointment.Paid;
            UpdateAppointment(appointment);
        }

        public void DeleteAppointment(int id)
        {
            var appointments = GetAppointments();
            var appointmentToRemove = appointments.FirstOrDefault(a => a.Id == id);
            if (appointmentToRemove is null)
            {
                return;
            }

            appointments.Remove(appointmentToRemove);
            SaveList(_appointmentsFilePath, appointments);
        }

        public List<User> GetUsers()
        {
            return LoadList<User>(_usersFilePath)
                .OrderBy(u => u.Name)
                .ToList();
        }

        public User? GetUserByEmail(string email) => GetUsers().FirstOrDefault(u => u.Email.Equals(email, System.StringComparison.OrdinalIgnoreCase));

        public bool AddUser(User user)
        {
            if (GetUserByEmail(user.Email) != null) return false;

            var users = GetUsers();
            user.Id = users.Any() ? users.Max(existingUser => existingUser.Id) + 1 : 1;
            users.Add(user);
            SaveList(_usersFilePath, users);
            return true;
        }

        private void EnsureSeedData()
        {
            if (!File.Exists(_usersFilePath))
            {
                SaveList(_usersFilePath, new List<User>
                {
                    new() { Id = 1, Name = "Demo User", Email = "demo@clinic.local", Password = "demo123" }
                });
            }

            if (!File.Exists(_appointmentsFilePath))
            {
                SaveList(_appointmentsFilePath, new List<Appointment>
                {
                    new()
                    {
                        Id = 1,
                        Name = "Nick Young",
                        Email = "nick@example.com",
                        Phone = "+380501112233",
                        Service = "Терапевтична консультація",
                        DoctorName = "Erica Lim",
                        Date = DateTime.Today.AddDays(1),
                        Time = "09:30",
                        UserEmail = "demo@clinic.local",
                        Status = AppointmentStatuses.Upcoming,
                        Paid = false,
                        Price = 600,
                        CreatedAt = DateTime.Today.AddDays(-2)
                    },
                    new()
                    {
                        Id = 2,
                        Name = "Alina Choo",
                        Email = "alina@example.com",
                        Phone = "+380661234567",
                        Service = "Стоматологічний огляд",
                        DoctorName = "Kevin Zane",
                        Date = DateTime.Today,
                        Time = "12:00",
                        UserEmail = "demo@clinic.local",
                        Status = AppointmentStatuses.Upcoming,
                        Paid = true,
                        Price = 850,
                        CreatedAt = DateTime.Today.AddDays(-1)
                    },
                    new()
                    {
                        Id = 3,
                        Name = "Maya Foster",
                        Email = "maya@example.com",
                        Phone = "+380971234567",
                        Service = "Дерматологічна консультація",
                        DoctorName = "Mina Park",
                        Date = DateTime.Today.AddDays(-1),
                        Time = "15:45",
                        UserEmail = "demo@clinic.local",
                        Status = AppointmentStatuses.Missed,
                        Paid = false,
                        Price = 700,
                        CreatedAt = DateTime.Today.AddDays(-4)
                    }
                });
            }
        }

        private static List<T> LoadList<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new List<T>();
            }

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        private static void SaveList<T>(string filePath, List<T> items)
        {
            var json = JsonSerializer.Serialize(items, JsonOptions);
            File.WriteAllText(filePath, json);
        }

        private static decimal ResolvePrice(string? service, decimal currentPrice)
        {
            if (currentPrice > 0)
            {
                return currentPrice;
            }

            return service switch
            {
                "Терапевтична консультація" => 600m,
                "Стоматологічний огляд" => 850m,
                "Дерматологічна консультація" => 700m,
                _ => 500m
            };
        }

        private static TimeSpan ParseTime(string? value)
        {
            return TimeSpan.TryParse(value, out var parsed)
                ? parsed
                : TimeSpan.Zero;
        }
    }
}