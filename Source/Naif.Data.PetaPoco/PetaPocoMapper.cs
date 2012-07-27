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

        public ColumnInfo GetColumnInfo(PropertyInfo pocoProperty)
        {
            ColumnInfo column = new ColumnInfo();
            object[] columnNameAttributes = pocoProperty.GetCustomAttributes(typeof(ColumnNameAttribute), true);
            if (columnNameAttributes.Length > 0)
            {
                var columnNameAttribute = (ColumnNameAttribute)columnNameAttributes[0];
                column.ColumnName = columnNameAttribute.ColumnName;
            }

            return column;
        }

        public TableInfo GetTableInfo(Type pocoType)
        {
            //Table Name
            TableInfo table = new TableInfo();
            table.TableName = Util.GetTableName(pocoType, table.TableName + "s");

            table.TableName = _tablePrefix + table.TableName;

            //Primary Key
            table.PrimaryKey = Util.GetPrimaryKeyName(pocoType);

            table.AutoIncrement = true;
            return table;
        }

        public Func<object, object> GetToDbConverter(PropertyInfo SourceProperty)
        {
            return null;
        }

        public Func<object, object> GetFromDbConverter(PropertyInfo TargetProperty, Type SourceType)
        {
            return null;
        }

        #endregion
    }
}