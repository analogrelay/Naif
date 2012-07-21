using System;
using System.Linq;
using System.Reflection;

namespace Naif.TestUtilities
{
    public static class Util
    {

        public static object GetPrivateMember<TInstance, TField>(TInstance instance, string fieldName)
        {
            Type type = typeof(TInstance);

            BindingFlags privateBindings = BindingFlags.NonPublic | BindingFlags.Instance;

            // retrive private field from class
            FieldInfo field = type.GetField(fieldName, privateBindings);

            return (TField)field.GetValue(instance);
        }

    }
}
