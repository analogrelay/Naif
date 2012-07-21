using System;

using System.Configuration;
using Naif.Core.Caching;
using Raven.Client;
using Naif.Core.Data;
using Naif.Core.Contracts;

namespace Naif.Data.RavenDB
{
    public class RavenDbDataContext : IDataContext
    {
        #region Private Members

        private readonly ICacheProvider _cache;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public RavenDbDataContext(ICacheProvider cache)
            : this(ConfigurationManager.ConnectionStrings[0].Name, cache)
        {
        }

        public RavenDbDataContext(string connectionStringName, ICacheProvider cache)
        {
            Requires.NotNullOrEmpty("connectionString", connectionStringName);
            Requires.NotNull("cache", cache);

            if (!RavenDbDocumentStore.IsInitialized)
            {
                RavenDbDocumentStore.Initialize(connectionStringName);
            }
            _documentSession = RavenDbDocumentStore.Instance.OpenSession();

            _cache = cache;
        }

        #endregion

        #region Implementation of IDataContext

        public void BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void Commit()
        {
            _documentSession.SaveChanges();
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            return new RavenDbRepository<T>(_documentSession, _cache);
        }

        public void RollbackTransaction()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            if (_documentSession != null)
            {
                _documentSession.Dispose();
            }
        }

        #endregion
    }
}
