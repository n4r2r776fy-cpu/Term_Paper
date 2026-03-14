using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentSystem.Models;

public class Appointment
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public string? PatientName { get; set; }
    public DateTime AppointmentDate { get; set; }
    
    [ForeignKey("DoctorId")]
    public Doctor? Doctor { get; set; }
}