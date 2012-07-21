using System;

namespace Naif.Core.Data
{
    public interface IDataContext : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class;

        void BeginTransaction();
        void Commit();
        void RollbackTransaction();
    }
}