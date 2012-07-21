using System;
using System.Collections.Generic;

using Naif.Core.Contracts;
using PetaPoco;
using Naif.Core.Data;
using Naif.Core.Caching;

namespace Naif.Data.PetaPoco
{
    public class PetaPocoRepository<T> : RepositoryBase<T> where T : class
    {
        #region Private Members

        private readonly Database _database;

        #endregion

        #region Constructors

        public PetaPocoRepository(Database database, ICacheProvider cache) : base(cache)
        {
            Requires.NotNull("database", database);

            _database = database;
        }

        #endregion

        protected override void AddInternal(T item)
        {
            _database.Insert(item);
        }

        protected override void DeleteInternal(T item)
        {
            _database.Delete(item);
        }

        protected override IEnumerable<T> GetAllInternal()
        {
            return _database.Fetch<T>("");
        }

        protected override T GetByIdInternal<TProperty>(TProperty id)
        {
            return _database.SingleOrDefault<T>(id);
        }

        protected override IEnumerable<T> GetByPropertyInternal<TProperty>(string propertyName, TProperty propertyValue)
        {
            return _database.Query<T>(String.Format("WHERE {0} = @0", propertyName), propertyValue);
        }

        protected override void UpdateInternal(T item)
        {
            _database.Update(item);
        }
    }
}
