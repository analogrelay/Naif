using System;
using System.Linq;
using System.Threading;

namespace Naif.Core.Locks
{
    public class SimpleLock
    {
        private readonly object _padlock = new object();

        public void AquireReadLock(Action action)
        {
            lock (_padlock)
            {
                action();
            }
        }

        public T AquireReadLock<T>(Func<T> func)
        {
            lock (_padlock)
            {
                return func();
            }
        }

        public void AquireWriteLock(Action action)
        {
            lock (_padlock)
            {
                action();
            }
        }

        public T AquireWriteLock<T>(Func<T> func)
        {
            lock (_padlock)
            {
                return func();
            }
        }

        public void EnterReadLock()
        {
            Monitor.Enter(_padlock);
        }

        public void ExitReadLock()
        {
            Monitor.Exit(_padlock);
        }
    }
}
