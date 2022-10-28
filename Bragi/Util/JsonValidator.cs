using BRAGI.Bragi;
using BRAGI.Valhalla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BRAGI.Util;

public static class JsonValidator
{
    public static bool CheckParameters<T>(JsonObject? parameters)
    {
        if (parameters == null) throw new InvalidParameterException("");
        foreach (PropertyInfo property in GetPublicProperties(typeof(T)))
        {
            if (property.GetCustomAttribute<OptionalParameter>() != null) continue;
            if (!parameters.ContainsKey(property.Name))
            {
                throw new InvalidParameterException(property.Name);
            }
        }
        return true;
    }

    private static IEnumerable<PropertyInfo> GetPublicProperties(this Type type)
    {
        if (!type.IsInterface)
            return type.GetProperties();

        return (new Type[] { type })
               .Concat(type.GetInterfaces())
               .SelectMany(i => i.GetProperties());
    }
}
