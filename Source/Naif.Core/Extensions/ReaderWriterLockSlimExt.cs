using System;
using System.Linq;
using System.Threading;

namespace Naif.Core.Extensions
{
    public static class ReaderWriterLockSlimExt
    {
        public static void AquireReadLock(this ReaderWriterLockSlim rwlock, Action action)
        {
            try 
            {
                rwlock.EnterReadLock();

                action();
            }
            finally 
            {
                rwlock.ExitReadLock();
            }
        }

        public static T AquireReadLock<T>(this ReaderWriterLockSlim rwlock, Func<T> func)
        {
            try
            {
                rwlock.EnterReadLock();

                return func();
            }
            finally
            {
                rwlock.ExitReadLock();
            }
        }

        public static void AquireWriteLock(this ReaderWriterLockSlim rwlock, Action action)
        {
            try
            {
                rwlock.EnterWriteLock();

                action();
            }
            finally
            {
                rwlock.ExitWriteLock();
            }
        }

        public static T AquireWriteLock<T>(this ReaderWriterLockSlim rwlock, Func<T> func)
        {
            try
            {
                rwlock.EnterWriteLock();

                return func();
            }
            finally
            {
                rwlock.ExitWriteLock();
            }
        }
    }
}
