using System;
using System.Collections.Generic;

namespace Mobile.BuildTools.Utils
{
    public static class PrimitivesExtensions
    {
        public static string GetStandardTypeName(this Type type) =>
            _primitives.ContainsKey(type) ? _primitives[type] : type.FullName;

        private static readonly IDictionary<Type, string> _primitives = new Dictionary<Type, string>
        {
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(string), "string" }
        };
    }
}
