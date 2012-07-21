using System;
using System.Linq;

namespace Naif.Core.ComponentModel
{
    public class PrimaryKeyAttribute : Attribute
    {
        public PrimaryKeyAttribute(string keyField)
        {
            KeyField = keyField;
        }
        public string KeyField { get; set; }
    }
}
