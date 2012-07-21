using System;

using System.Configuration;
using Naif.Core.Caching;
using Raven.Client;
using Naif.Core.Data;
using Naif.Core.Contracts;

namespace Naif.Data.RavenDB
{
    public class RavenDBDataContext : IDataContext
    {
        #region Private Members

        private readonly ICacheProvider _cache;
        private readonly IDocumentSession _documentSession;

        #endregion

        #region Constructors

        public RavenDBDataContext(ICacheProvider cache)
            : this(ConfigurationManager.ConnectionStrings[0].Name, cache)
        {
        }

        public RavenDBDataContext(string connectionStringName, ICacheProvider cache)
        {
            Requires.NotNullOrEmpty("connectionString", connectionStringName);
            Requires.NotNull("cache", cache);

            if (!RavenDBDocumentStore.IsInitialized)
            {
                RavenDBDocumentStore.Initialize(connectionStringName);
            }
            _documentSession = RavenDBDocumentStore.Instance.OpenSession();

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
            return new RavenDBRepository<T>(_documentSession, _cache);
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
