using System;
using System.Reflection;

using PetaPoco;
using Naif.Core.ComponentModel;

namespace Naif.Data.PetaPoco
{
    public class PetaPocoMapper : IMapper
    {
        private readonly string _tablePrefix;

        public PetaPocoMapper(string tablePrefix)
        {
            _tablePrefix = tablePrefix;
        }

        #region Implementation of IMapper

        public void GetTableInfo(Type t, TableInfo ti)
        {
            //Table Name
            ti.TableName = Util.GetTableName(t, ti.TableName + "s");

            ti.TableName = _tablePrefix + ti.TableName;

            //Primary Key
            ti.PrimaryKey = Util.GetPrimaryKeyName(t);

            ti.AutoIncrement = true;
        }

        public bool MapPropertyToColumn(PropertyInfo pi, ref string columnName, ref bool resultColumn)
        {
            object[] columnNameAttributes = pi.GetCustomAttributes(typeof(ColumnNameAttribute), true);
            if (columnNameAttributes.Length > 0)
            {
                var columnNameAttribute = (ColumnNameAttribute)columnNameAttributes[0];
                columnName = columnNameAttribute.ColumnName;
            }

            return true;
        }

        public Func<object, object> GetFromDbConverter(PropertyInfo pi, Type SourceType)
        {
            return null;
        }

        public Func<object, object> GetToDbConverter(Type SourceType)
        {
            return null;
        }
        #endregion
    }
}