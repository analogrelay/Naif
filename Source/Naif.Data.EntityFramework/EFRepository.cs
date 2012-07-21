using System;
using System.Collections.Generic;
using System.Linq;
using Naif.Core.Data;

namespace Naif.Data.EntityFramework
{
    public class EFRepository<T> : RepositoryBase<T> where T : class
    {

        protected override void AddInternal(T item)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        protected override void DeleteInternal(T item)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        protected override IEnumerable<T> GetAllInternal()
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        protected override T GetByIdInternal(int id)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        protected override void UpdateInternal(T item)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
    }
}
