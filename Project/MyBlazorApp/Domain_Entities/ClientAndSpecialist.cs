using System;
using Notification_System;

namespace Domain_Entities
{
    // Клієнт наслідує Person і реалізує IObserver
    public class Client : Person, IObserver
    {
        private DateTime dateOfBirth;

        public DateTime DateOfBirth { get => dateOfBirth; set => dateOfBirth = value; }
        public DateTime GetDateOfBirth() => DateOfBirth;

        public void Update(string message)
        {
            Console.WriteLine($"Клієнт {Name} отримав сповіщення: {message}");
        }
    }

    // Спеціаліст наслідує Person
    public class Specialist : Person
    {
        private string specialization;
        private int experience;

        public string Specialization { get => specialization; set => specialization = value; }
        public int Experience { get => experience; set => experience = value; }

        public string GetSpecialization() => Specialization;
    }
}