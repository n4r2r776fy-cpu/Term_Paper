namespace Domain_Entities
{
    public abstract class Person
    {
        private int id;
        private string name;
        private string phone;
        private string email;

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Phone { get => phone; set => phone = value; }
        public string Email { get => email; set => email = value; }

        public int GetId() => Id;
    }
}