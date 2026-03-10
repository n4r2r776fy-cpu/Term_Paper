using System.Collections.Generic;

namespace Repositories
{
    // Використовуємо узагальнення (Generics) <T> для універсальності
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Save(T entity);
        void Delete(int id);
    }
}