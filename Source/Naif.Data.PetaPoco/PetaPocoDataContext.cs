using System;
using System.Configuration;
using System.Data.SqlClient;
using Naif.Core.Caching;
using Naif.Core.Data;
using PetaPoco;
using Naif.Core.Contracts;

namespace Naif.Data.PetaPoco
{
    public class PetaPocoDataContext : IDataContext
    {
        #region Private Members

        private readonly ICacheProvider _cache;
        private readonly Database _database;

        #endregion

        #region Constructors

        public PetaPocoDataContext(ICacheProvider cache) 
            : this(ConfigurationManager.ConnectionStrings[0].Name, String.Empty, cache)
        {
        }

        public PetaPocoDataContext(string connectionString, ICacheProvider cache)
            : this(connectionString, String.Empty, cache)
        {
        }

        public PetaPocoDataContext(string connectionString, string tablePrefix, ICacheProvider cache)
        {
            Requires.NotNullOrEmpty("connectionString", connectionString);
            Requires.NotNull("cache", cache);

            _database = new Database(connectionString, "System.Data.SqlClient");
            _cache = cache; 
            Database.Mapper = new PetaPocoMapper(tablePrefix);
        }

        #endregion

        #region Implementation of IDataContext

        public void BeginTransaction()
        {
            _database.BeginTransaction();
        }

        public void Commit()
        {
            _database.CompleteTransaction();
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            return new PetaPocoRepository<T>(_database, _cache);
        }

        public void RollbackTransaction()
        {
            _database.AbortTransaction();
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            _database.Dispose();
        }
        #endregion
    }
}