using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Naif.Core.Locks;

namespace Naif.Core.Collections
{
    /// <summary>
    /// SynchronizedList is a thread-safe implementation of the IList interface
    /// </summary>
    /// <typeparam name="T">The type of the values in the list.</typeparam>
    public class SynchronizedList<T> : IList<T>
    {
        private readonly List<T> _list;
        private readonly SimpleLock _lock = new SimpleLock();

        #region Constructors

        public SynchronizedList()
        {
            _list = new List<T>();
        }

        #endregion

        #region IList<T> Members

        public T this[int index]
        {
            get { return _lock.AquireReadLock(() => _list[index]); }
            set { _lock.AquireWriteLock(() => _list[index] = value); }
        }

        public int IndexOf(T item)
        {
            return _lock.AquireReadLock(() => _list.IndexOf(item));
        }

        public void Insert(int index, T item)
        {
            _lock.AquireWriteLock(() => _list.Insert(index, item));
        }

        public void RemoveAt(int index)
        {
            _lock.AquireWriteLock(() => _list.RemoveAt(index));
        }

        #endregion

        #region ICollection<T> Members

        public int Count
        {
            get { return _lock.AquireReadLock(() => _list.Count); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(T item)
        {
            _lock.AquireWriteLock(() => _list.Add(item));
        }

        public void Clear()
        {
            _lock.AquireWriteLock(() => _list.Clear());
        }

        public bool Contains(T item)
        {
            return _lock.AquireReadLock(() => _list.Contains(item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _lock.AquireWriteLock(() => _list.CopyTo(array, arrayIndex));
        }

        public bool Remove(T item)
        {
            return _lock.AquireWriteLock(() => _list.Remove(item));
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _lock.AquireReadLock(() => _list.GetEnumerator());
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return _lock.AquireReadLock(() => _list.GetEnumerator());
        }

        #endregion
    }
}
