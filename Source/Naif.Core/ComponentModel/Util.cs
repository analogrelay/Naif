using System;

namespace Naif.Core.ComponentModel
{
    public static class Util
    {
        public static string GetCacheKey(Type type)
        {
            var cacheKey = String.Empty;

            var cacheableAttribute = GetAttribute<CacheableAttribute>(type);
            if (cacheableAttribute != null)
            {
                if (!String.IsNullOrEmpty(cacheableAttribute.CacheKey))
                {
                    cacheKey = cacheableAttribute.CacheKey;
                }
                else
                {
                    cacheKey = String.Format("OR_{0}", type.Name);
                }
            }

            return cacheKey;
        }

        public static bool GetIsCacheable(Type type)
        {
            return (GetAttribute<CacheableAttribute>(type) != null);
        }

        public static string GetPrimaryKeyName(Type type)
        {
            var primaryKeyName = String.Empty;

            var primaryKeyAttribute = GetAttribute<PrimaryKeyAttribute>(type);
            if (primaryKeyAttribute != null)
            {
                primaryKeyName = primaryKeyAttribute.KeyField;
            }

            return primaryKeyName;
        }

        public static string GetTableName(Type type)
        {
            return GetTableName(type, String.Empty);
        }

        public static string GetTableName(Type type, string defaultName)
        {
            var tableName = defaultName;

            var tableNameAttribute = GetAttribute<TableNameAttribute>(type);
            if (tableNameAttribute != null)
            {
                tableName = tableNameAttribute.TableName;
            }

            return tableName;
        }

        private static TAttribute GetAttribute<TAttribute>(Type type)
        {
            TAttribute attribute = default(TAttribute);

            object[] tableNameAttributes = type.GetCustomAttributes(typeof(TAttribute), true);
            if (tableNameAttributes.Length > 0)
            {
                attribute = (TAttribute)tableNameAttributes[0];
            }

            return attribute;
        }
    }
}
