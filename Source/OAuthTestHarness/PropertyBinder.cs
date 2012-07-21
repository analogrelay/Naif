using System;
using System.Reflection;
using System.Diagnostics;

namespace OAuthTestHarness
{
    public class PropertyBinder
    {
        public object TheObject { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public string Name
        {
            get
            {
                Debug.Assert(PropertyInfo != null);
                return PropertyInfo.Name;
            }
        }

        public object Value
        {
            get
            {
                try
                {
                    Debug.Assert(PropertyInfo != null);
                    return PropertyInfo.GetValue(TheObject, null) != null ? PropertyInfo.GetValue(TheObject, null).ToString() : "(null)";
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
        }

        public bool HasChildren
        {
            get
            {
                Debug.Assert(PropertyInfo != null);
                return !PropertyInfo.PropertyType.IsValueType;// && !(TheObject is string);
            }
        }
    }
}
