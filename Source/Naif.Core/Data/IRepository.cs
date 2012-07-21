using System.Collections.Generic;

namespace Naif.Core.Data
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();

        T GetById<TProperty>(TProperty id);
        IEnumerable<T> GetByProperty<TProperty>(string propertyName, TProperty propertyValue);

        void Add(T item);
        void Delete(T item);
        void Update(T item);
    }
}


