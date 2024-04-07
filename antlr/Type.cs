using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace antlr
{
    public enum Type
    {
        Unknown,
        Int,
        Float,
        Bool,
        String,
        Error,
    }

    ////
    /// Get default value for a type
    /// 
    public static class TypeExtensions
    {
        public static object DefaultValue(this Type type)
        {
            switch (type)
            {
                case Type.Int:
                    return 0;
                case Type.Float:
                    return 0.0;
                case Type.Bool:
                    return false;
                case Type.String:
                    return "";
                default:
                    throw new InvalidOperationException("Unknown type");
            }
        }

        public static Type FromString(this string value)
        {
            switch (value)
            {
                case "int":
                    return Type.Int;
                case "float":
                    return Type.Float;
                case "bool":
                    return Type.Bool;
                case "string":
                    return Type.String;
                default:
                    return Type.Unknown;
            }
        }
    }
}
