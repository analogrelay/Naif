using System;
using System.Reflection;

using Raven.Client;
using Raven.Client.Embedded;
using Naif.Core.ComponentModel;

namespace Naif.Data.RavenDB
{
    public class RavenDBDocumentStore
    {
        private static IDocumentStore instance;

        public static IDocumentStore Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new InvalidOperationException("The Document Store must be initialized before it can be used");
                }
                return instance;
            }
        }

        public static bool IsInitialized
        {
            get
            {
                return instance != null;
            }
        }

        public static IDocumentStore Initialize(string connectionStringName)
        {
            instance = new EmbeddableDocumentStore
            {
                ConnectionStringName = connectionStringName
            };
            instance.Conventions.IdentityPartsSeparator = "-";
            instance.Conventions.FindIdentityProperty = FindIdentityProperty;
            instance.Initialize();
            return instance;
        }

        private static bool FindIdentityProperty(PropertyInfo propertyInfo)
        {
            return (propertyInfo.Name == Util.GetPrimaryKeyName(propertyInfo.DeclaringType));
        }
    }
}