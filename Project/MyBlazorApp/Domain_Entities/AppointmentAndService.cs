using System;

namespace Domain_Entities
{
    public class Appointment
    {
        private int appointmentId;
        private DateTime date;
        private TimeSpan time;
        private string status;

        public int AppointmentId { get => appointmentId; set => appointmentId = value; }
        public DateTime Date { get => date; set => date = value; }
        public TimeSpan Time { get => time; set => time = value; }
        public string Status { get => status; set => status = value; }

        // Ось ці два рядки дуже важливі!
        public Client Client { get; set; }
        public Specialist Specialist { get; set; }

        public string GetStatus() => Status;
        public void SetStatus(string newStatus) => Status = newStatus;
    }

    public class Service
    {
        private int serviceId;
        private string name;
        private double price;
        private int duration;

        public int ServiceId { get => serviceId; set => serviceId = value; }
        public string Name { get => name; set => name = value; }
        public double Price { get => price; set => price = value; }
        public int Duration { get => duration; set => duration = value; }
    }
}
