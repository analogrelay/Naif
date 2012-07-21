using System;
using System.Linq;
using System.Globalization;
using System.Windows.Data;

namespace OAuthTestHarness
{
    public class ObjectPropertiesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return from p in value.GetType().GetProperties()
                   where p.CanRead && p.GetIndexParameters().Count() == 0
                   select new PropertyBinder
                   {
                       PropertyInfo = p,
                       TheObject = value
                   };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
