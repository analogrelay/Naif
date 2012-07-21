using System;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using System.Linq.Expressions;
using Naif.Core.Contracts;
using Naif.Core.Resources;

namespace Naif.Core.ComponentModel
{
    public static class ContainerExt
    {
        #region Register Overloads

        public static void Register<T>(this Container container, string key, Func<Container, Type, string, object> builder)
        {
            container.Register(typeof(T), key, builder);
        }

        public static void Register<T>(this Container container, Func<Container, Type, string, object> builder)
        {
            container.Register(typeof(T), null, builder);
        }

        #endregion

        #region Instance Methods

        public static void RegisterInstance(this Container container, Type type, string key, object instance)
        {
            container.Register(type, key, (c, t, k) => instance);
        }

        public static void RegisterInstance<T>(this Container container, string key, object instance)
        {
            container.Register(typeof(T), key, (c, t, k) => instance);
        }

        public static void RegisterInstance<T>(this Container container, object instance)
        {
            container.Register(typeof(T), null, (c, t, k) => instance);
        }

        #endregion

        #region Singleton Methods

        public static void RegisterSingleton(this Container container, Type type, string key, object instance)
        {
            var disposableInstance = instance as IDisposable;
            if (disposableInstance != null)
            {
                container.Disposing += (sender, eventArgs) =>
                {
                    if (disposableInstance != null)
                    {
                        try
                        {
                            disposableInstance.Dispose();
                        }
                        finally
                        {
                            disposableInstance = null;
                        }
                    }
                };
            }
            container.Register(type, key, (c, t, k) => instance);
        }

        public static void RegisterSingleton<T>(this Container container, string key, object instance)
        {
            container.RegisterSingleton(typeof(T), key, instance);
        }

        public static void RegisterSingleton<T>(this Container container, object instance)
        {
            container.RegisterSingleton(typeof(T), null, instance);
        }

        #endregion

        #region Type Methods

        private static Func<Container, Type, string, object> GetCreator(Type type)
        {
            Requires.NotNull("type", type);

            var constructors = type.GetConstructors();
            if (constructors == null || constructors.Length == 0)
            {
                throw new ActivationException(string.Format(CommonErrors.NoConstructorFound, type.FullName));
            }

            var constructor = constructors[0];

            var containerParameter = Expression.Parameter(typeof(Container), "container");
            var typeParameter = Expression.Parameter(typeof(Type), "type");
            var keyParameter = Expression.Parameter(typeof(string), "key");

            var creator = Expression.Lambda(typeof(Func<Container, Type, string, object>),
                Expression.New(constructor, constructor.GetParameters().Select(p => (Expression)Expression.Call(containerParameter, "GetInstance", new[] { p.ParameterType }))),
                containerParameter,
                typeParameter,
                keyParameter);
            return (Func<Container, Type, string, object>)creator.Compile();
        }

        public static void RegisterType(this Container container, Type serviceType, string key, Type implementationType)
        {
            container.Register(serviceType, key, GetCreator(implementationType));
        }

        public static void RegisterType<TService, TImplementation>(this Container container, string key)
        {
            container.RegisterType(typeof(TService), key, typeof(TImplementation));
        }

        public static void RegisterType<TService, TImplementation>(this Container container)
        {
            container.RegisterType(typeof(TService), null, typeof(TImplementation));
        }

        #endregion
    }
}
