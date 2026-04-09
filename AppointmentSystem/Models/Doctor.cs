namespace AppointmentSystem.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        
        // ДОДАЙ ЦЕЙ РЯДОК:
        public string Bio { get; set; } = string.Empty;
    }
}