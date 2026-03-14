using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using MyBlazorApp.Models;

namespace MyBlazorApp.Services
{
    public class DataService
    {
        private static readonly string[] StandardTimeSlots =
        {
            "09:00", "09:30", "10:00", "10:30", "11:00", "11:30",
            "12:00", "12:30", "13:00", "13:30", "14:00", "14:30",
            "15:00", "15:30", "16:00", "16:30", "17:00"
        };

        private static readonly Dictionary<string, string[]> ServicesBySpecialty = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Терапевт"] = new[] { "Терапевтична консультація" },
            ["Стоматолог"] = new[] { "Стоматологічний огляд" },
            ["Дерматолог"] = new[] { "Дерматологічна консультація" }
        };

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

        public IReadOnlyList<string> GetServicesForDoctor(string? doctorName)
        {
            var specialty = GetDoctors()
                .FirstOrDefault(doctor => doctor.Name.Equals(doctorName ?? string.Empty, StringComparison.OrdinalIgnoreCase))
                ?.Specialty;

            if (string.IsNullOrWhiteSpace(specialty) || !ServicesBySpecialty.TryGetValue(specialty, out var services))
            {
                return Array.Empty<string>();
            }

            return services;
        }

        public IReadOnlyList<string> GetAvailableTimeSlots(string? doctorName, DateTime date, int? excludingAppointmentId = null)
        {
            if (string.IsNullOrWhiteSpace(doctorName))
            {
                return Array.Empty<string>();
            }

            return StandardTimeSlots
                .Where(time => IsFutureOrCurrentSlot(date, time))
                .Where(time => IsDoctorAvailable(doctorName, date, time, excludingAppointmentId))
                .ToList();
        }

        public bool IsDoctorAvailable(string? doctorName, DateTime date, string? time, int? excludingAppointmentId = null)
        {
            if (string.IsNullOrWhiteSpace(doctorName) || string.IsNullOrWhiteSpace(time))
            {
                return false;
            }

            var normalizedDate = date.Date;
            var normalizedTime = time.Trim();

            return GetAppointments().All(appointment =>
                appointment.Id == excludingAppointmentId
                || appointment.Status == AppointmentStatuses.Cancelled
                || !appointment.DoctorName.Equals(doctorName, StringComparison.OrdinalIgnoreCase)
                || appointment.Date.Date != normalizedDate
                || !string.Equals(appointment.Time, normalizedTime, StringComparison.OrdinalIgnoreCase));
        }

        public void SaveAppointment(Appointment appointment)
        {
            NormalizeAppointment(appointment);

            if (!IsDoctorAvailable(appointment.DoctorName, appointment.Date, appointment.Time))
            {
                throw new InvalidOperationException("Обраний час уже зайнятий. Будь ласка, виберіть інший слот.");
            }

            var allowedServices = GetServicesForDoctor(appointment.DoctorName);
            if (IsKnownService(appointment.Service) && allowedServices.Count > 0 && !allowedServices.Contains(appointment.Service ?? string.Empty, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Обрана послуга недоступна для цього лікаря.");
            }

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
            NormalizeAppointment(appointment);

            var appointments = GetAppointments();
            var index = appointments.FindIndex(a => a.Id == appointment.Id);
            if (index < 0)
            {
                return;
            }

            if (!IsDoctorAvailable(appointment.DoctorName, appointment.Date, appointment.Time, appointment.Id))
            {
                throw new InvalidOperationException("Обраний час уже зайнятий. Будь ласка, виберіть інший слот.");
            }

            var allowedServices = GetServicesForDoctor(appointment.DoctorName);
            if (IsKnownService(appointment.Service) && allowedServices.Count > 0 && !allowedServices.Contains(appointment.Service ?? string.Empty, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Обрана послуга недоступна для цього лікаря.");
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

        private static bool IsFutureOrCurrentSlot(DateTime date, string time)
        {
            if (!TimeSpan.TryParse(time, out var parsedTime))
            {
                return false;
            }

            var appointmentMoment = date.Date.Add(parsedTime);
            return appointmentMoment >= DateTime.Now;
        }

        private static bool IsKnownService(string? service) => ServicesBySpecialty.Values
            .SelectMany(items => items)
            .Contains(service ?? string.Empty, StringComparer.OrdinalIgnoreCase);

        private static void NormalizeAppointment(Appointment appointment)
        {
            appointment.Name = appointment.Name.Trim();
            appointment.Email = appointment.Email.Trim();
            appointment.Phone = appointment.Phone.Trim();
            appointment.DoctorName = appointment.DoctorName.Trim();
            appointment.Service = appointment.Service?.Trim();
            appointment.Time = appointment.Time.Trim();
            appointment.Date = appointment.Date.Date;
        }
    }
}