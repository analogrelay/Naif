using System;
using System.Collections.Generic;
using Naif.Core.Contracts;
using Raven.Client;
using Naif.Core.Data;
using Naif.Core.Caching;

namespace Naif.Data.RavenDB
{
    public class RavenDbRepository<T> : RepositoryBase<T> where T : class
    {
        private readonly IDocumentSession _database;

        #region Constructors

        public RavenDbRepository(IDocumentSession database, ICacheProvider cache) : base(cache)
        {
            Requires.NotNull("database", database);

            _database = database;
        }

        #endregion

        protected override void AddInternal(T item)
        {
            _database.Store(item);
        }

        protected override void DeleteInternal(T item)
        {
            _database.Delete(item);
        }

        protected override IEnumerable<T> GetAllInternal()
        {
            return _database.Query<T>();
        }

        protected override T GetByIdInternal<TProperty>(TProperty id)
        {
            foreach (T item in _database.Query<T>())
            {
                if (CompareTo(GetPrimaryKey<TProperty>(item), id) == 0)
                {
                    return item;
                }
            }
            return null;
        }

        protected override IEnumerable<T> GetByPropertyInternal<TProperty>(string propertyName, TProperty propertyValue)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        protected override void UpdateInternal(T item)
        {
            _database.Store(item);
        }
    }
}
