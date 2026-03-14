using System.ComponentModel.DataAnnotations;

namespace MyBlazorApp.Models
{
    public static class AppointmentStatuses
    {
        public const string Upcoming = "Upcoming";
        public const string Completed = "Completed";
        public const string Missed = "Missed";
        public const string Cancelled = "Cancelled";
    }

    public class User 
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ім'я є обов'язковим")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Email є обов'язковим")]
        [EmailAddress(ErrorMessage = "Неправильний формат Email")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Пароль є обов'язковим")]
        [MinLength(6, ErrorMessage = "Пароль має містити принаймні 6 символів")]
        public string Password { get; set; } = "";
    }

    public class Appointment 
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ім'я є обов'язковим")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Email є обов'язковим")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Телефон є обов'язковим")]
        public string Phone { get; set; } = "";

        [Required(ErrorMessage = "Будь ласка, виберіть послугу")]
        public string? Service { get; set; }

        [Required(ErrorMessage = "Будь ласка, виберіть лікаря")]
        public string DoctorName { get; set; } = "";

        [Required]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Час є обов'язковим")]
        public string Time { get; set; } = "";
    

        public string? UserEmail { get; set; }

            public string Status { get; set; } = AppointmentStatuses.Upcoming;

            public bool Paid { get; set; }

            public decimal Price { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

        public class Doctor
        {
            public string Name { get; set; } = "";
            public string Specialty { get; set; } = "";
            public string AvatarPath { get; set; } = "";
        }
}