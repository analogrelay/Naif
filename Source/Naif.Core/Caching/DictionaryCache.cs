using System;
using System.Linq;
using Naif.Core.Collections;

namespace Naif.Core.Caching
{
    public class DictionaryCache : ICacheProvider
    {
        #region Private Members

        private readonly SynchronizedDictionary<string, object> _dictionary;

        #endregion

        #region Constructors

        public DictionaryCache()
        {
            _dictionary = new SynchronizedDictionary<string, object>();
        }

        #endregion

        #region ICacheProvider Members

        public object this[string key]
        {
            get
            {
                return _dictionary[key];
            }
            set
            {
                _dictionary[key] = value;
            }
        }

        public object Get(string key)
        {
            return _dictionary[key];
        }

        public void Insert(string key, object value, DateTime absoluteExpiration)
        {
            _dictionary.Add(key, value);
        }

        public void Insert(string key, object value)
        {
            _dictionary.Add(key, value);
        }

        public void Remove(string key)
        {
            _dictionary.Remove(key);
        }

        #endregion
    }
}
