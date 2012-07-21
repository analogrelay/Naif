using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using Naif.Core.Caching;
using Naif.Core.ComponentModel;
using Naif.Core.Contracts;

namespace Naif.Core.Data
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        #region Private Members

        private readonly ICacheProvider _cache;

        #endregion

        #region Constructors

        public RepositoryBase(ICacheProvider cache)
        {
            Requires.NotNull("cache", cache);

            _cache = cache;
        }

        #endregion

        #region IRepository<T> Implementation

        public IEnumerable<T> GetAll()
        {
            IEnumerable<T> items;

            if (IsCacheable)
            {
                //Check if the collection is currently cached
                items = _cache.GetCachedObject<IEnumerable<T>>(CacheKey, GetAllInternal);
            }
            else
            {
                items = GetAllInternal();
            }
            return items;
        }

        public T GetById<TProperty>(TProperty id)
        {
            T item;

            if (IsCacheable)
            {
                //Look up in cached collection
                item = GetAll().SingleOrDefault(t => CompareTo(GetPrimaryKey<TProperty>(t), id) == 0);
            }
            else
            {
                item = GetByIdInternal<TProperty>(id);
            }

            return item;
        }

        public IEnumerable<T> GetByProperty<TProperty>(string propertyName, TProperty propertyValue)
        {
            IEnumerable<T> items;

            if (IsCacheable)
            {
                //Look up in cached collection
                items = GetAll().Where((t) => CompareTo(GetPropertyValue<TProperty>(t, propertyName), propertyValue) == 0);
            }
            else
            {
                items = GetByPropertyInternal(propertyName, propertyValue);
            }

            return items;
        }

        public void Add(T item)
        {
            AddInternal(item);
            ClearCache();
        }

        public void Delete(T item)
        {
            DeleteInternal(item);
            ClearCache();
        }

        public void Update(T item)
        {
            UpdateInternal(item);
            ClearCache();
        }

        #endregion

        #region Private Methods

        private void ClearCache()
        {
            if (IsCacheable)
            {
                _cache.Remove(CacheKey);
            }
        }

        private TProperty GetPropertyValue<TProperty>(Type modelType, T item, string propertyName)
        {
            var property = modelType.GetProperty(propertyName);

            return (TProperty)property.GetValue(item, null);
        }

        #endregion

        #region Protected Methods

        protected string CacheKey
        {
            get
            {
                return Util.GetCacheKey(typeof(T));
            }
        }

        protected int CompareTo<TProperty>(TProperty first, TProperty second)
        {
            Requires.IsTypeOf<IComparable>("first", first);
            Requires.IsTypeOf<IComparable>("second", second);

            var firstComparable = first as IComparable;
            var secondComparable = second as IComparable;

            return firstComparable.CompareTo(secondComparable);
        }

        protected TProperty GetPropertyValue<TProperty>(T item, string propertyName)
        {
            return GetPropertyValue<TProperty>(typeof(T), item, propertyName);
        }

        protected TProperty GetPrimaryKey<TProperty>(T item)
        {
            Type modelType = typeof(T);

            //Get the primary key
            var primaryKeyName = Util.GetPrimaryKeyName(modelType);

            return GetPropertyValue<TProperty>(modelType, item, primaryKeyName);
        }

        protected bool IsCacheable
        {
            get
            {
                return Util.GetIsCacheable(typeof(T));
            }
        }

        #endregion

        #region Abstract Methods

        protected abstract void AddInternal(T item);

        protected abstract void DeleteInternal(T item);

        protected abstract IEnumerable<T> GetAllInternal();

        protected abstract T GetByIdInternal<TProperty>(TProperty id);

        protected abstract IEnumerable<T> GetByPropertyInternal<TProperty>(string propertyName, TProperty propertyValue);

        protected abstract void UpdateInternal(T item);

        #endregion
    }
}
