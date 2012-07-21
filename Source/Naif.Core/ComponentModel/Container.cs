///
/// - Inspired by Simple Container (http://simplecontainer.codeplex.com)
/// 

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Naif.Core.Contracts;
using Naif.Core.Resources;
using Naif.Core.Collections;

namespace Naif.Core.ComponentModel
{
    public class Container : ServiceLocatorImplBase, IDisposable
    {
        private bool _disposed; 

        protected readonly SynchronizedDictionary<Type, SynchronizedDictionary<string, Func<Container, Type, string, object>>> _typeBuilders = new SynchronizedDictionary<Type, SynchronizedDictionary<string, Func<Container, Type, string, object>>>();

        public event EventHandler Disposing;

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            OnDisposing(EventArgs.Empty);
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            Requires.NotNull("serviceType", serviceType);

            var builders = _typeBuilders[serviceType];

            if (builders != null && builders.Count > 0)
            {
                Func<Container, Type, string, object> builder;
                if (string.IsNullOrEmpty(key) || !builders.TryGetValue(key, out builder) || builder == null)
                {
                    // fallback to empty key
                    builders.TryGetValue(string.Empty, out builder);
                }
                if (builder != null)
                {
                    return builder(this, serviceType, key);
                }
            }

            throw new ActivationException(string.Format(CommonErrors.ServiceNotFound, serviceType.AssemblyQualifiedName, key));
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            Requires.NotNull("serviceType", serviceType);

            var builders = _typeBuilders[serviceType];
            if (builders == null || builders.Count == 0)
            {
                yield break;
            }

            foreach (KeyValuePair<string, Func<Container, Type, string, object>> pair in builders)
            {
                yield return pair.Value(this, serviceType, pair.Key);
            }
        }

        protected void OnDisposing(EventArgs args)
        {
            if (Disposing != null)
            {
                Disposing(this, args);
            }
        }

        #endregion

        #region Public Methods

        public void Register(Type type, string key, Func<Container, Type, string, object> builder)
        {
            Requires.NotNull("builder", builder);

            SynchronizedDictionary<string, Func<Container, Type, string, object>> builders;
            if (!_typeBuilders.TryGetValue(type, out builders))
            {
                _typeBuilders.Add(type, builders = new SynchronizedDictionary<string, Func<Container, Type, string, object>>(StringComparer.OrdinalIgnoreCase));
            }
            builders[key ?? string.Empty] = builder;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (!_disposed)
            {
                try
                {
                    Dispose(true);
                }
                finally
                {
                    _disposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }

        #endregion

        #region Destructors

        ~Container()
        {
            Dispose(false);
        }
        #endregion
    }
}
