using System;
using System.Linq;

namespace Naif.Core.ComponentModel
{
    public class TableNameAttribute : Attribute
    {
        public TableNameAttribute(string tableName)
        {
            TableName = tableName;
        }
        public string TableName { get; set; }
    }
}
