using System.Collections.Generic;
using System.Linq;
using Domain_Entities;

namespace Repositories
{
    public class ClientRepository : IRepository<Client>
    {
        // Зберігаємо клієнтів у пам'яті (поки програма працює)
        private readonly List<Client> _clients = new List<Client>();

        public IEnumerable<Client> GetAll() => _clients;

        public Client GetById(int id) => _clients.FirstOrDefault(c => c.Id == id);

        public void Save(Client entity)
        {
            _clients.Add(entity);
        }

        public void Delete(int id)
        {
            var client = GetById(id);
            if (client != null)
            {
                _clients.Remove(client);
            }
        }
    }
}