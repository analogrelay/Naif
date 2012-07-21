using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Naif.Core.Locks;

namespace Naif.Core.Collections
{
    /// <summary>
    /// SynchronizedDictionary is a thread-safe implementation of the IDictionary interface
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    public class SynchronizedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _dictionary;
        private readonly SimpleLock _lock = new SimpleLock();

        #region Constructors

        public SynchronizedDictionary()
        {
            this._dictionary = new Dictionary<TKey, TValue>();
        }

        public SynchronizedDictionary(IEqualityComparer<TKey> comparer)
        {
            this._dictionary = new Dictionary<TKey, TValue>(comparer);
        }

        public SynchronizedDictionary(int capacity)
        {
            this._dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        #endregion

        #region IDictionary<TKey, TValue> Members

        public ICollection<TKey> Keys
        {
            get { return _lock.AquireReadLock(() => _dictionary.Keys); }
        }

        public ICollection<TValue> Values
        {
            get { return _lock.AquireWriteLock(() => _dictionary.Values); }
        }

        public TValue this[TKey key]
        {
            get { return _lock.AquireReadLock(() => _dictionary[key]); }
            set { _lock.AquireWriteLock(() => _dictionary[key] = value); }
        }

        public void Add(TKey key, TValue value)
        {
            _lock.AquireReadLock(() => _dictionary[key] = value);
        }

        public bool ContainsKey(TKey key)
        {
            return _lock.AquireReadLock(() => _dictionary.ContainsKey(key));
        }

        public bool Remove(TKey key)
        {
            return _lock.AquireWriteLock(() => _dictionary.Remove(key));
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            try
            {
                _lock.EnterReadLock();

                return _dictionary.TryGetValue(key, out value);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey, TValue>> Members

        public int Count
        {
            get { return _lock.AquireReadLock(() => _dictionary.Count); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _lock.AquireReadLock(() => _dictionary[item.Key] = item.Value);
        }

        public void Clear()
        {
            _lock.AquireWriteLock(() => _dictionary.Clear());
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _lock.AquireReadLock(() => _dictionary.Contains(item));
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _lock.AquireReadLock(() => _dictionary.ToArray().CopyTo(array, arrayIndex));
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return _lock.AquireWriteLock(() => _dictionary.Remove(item.Key));
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey, TValue>> Members

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _lock.AquireReadLock(() => _dictionary.GetEnumerator());
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _lock.AquireReadLock(() => _dictionary.GetEnumerator());
        }

        #endregion

    }
}
