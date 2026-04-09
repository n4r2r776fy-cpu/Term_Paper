public class Appointment
{
    public int Id { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string ClientPhone { get; set; } = string.Empty;
    public int DoctorId { get; set; }
    // ПЕРЕВІР ЦЮ НАЗВУ:
    public DateTime DateTime { get; set; } 
}