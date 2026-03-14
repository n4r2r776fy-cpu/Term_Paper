using System.ComponentModel.DataAnnotations;

namespace MyBlazorApp.Models
{
public class User
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
}

    public class Appointment
    {
        public int Id { get; set; }
        [Required]
        public string? UserEmail { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required, EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Phone { get; set; }
        [Required]
        public string? Service { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string? Time { get; set; }
    }
}