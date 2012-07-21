using System;
using System.Data.Entity;
using System.Linq;
using Naif.Core.Data;

namespace Naif.Data.EntityFramework
{
    public class EFDataContext : IDataContext
    {
        private DbContext _context;

        public IRepository<T> GetRepository<T>() where T : class
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        public void BeginTransaction()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        public void Commit()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        public void RollbackTransaction()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
    }
}
