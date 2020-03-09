using System;
using Mobile.BuildTools.Handlers;

namespace Mobile.BuildTools.Models.Secrets
{
    public enum PropertyType
    {
        [PropertyTypeMapping(typeof(string), "\"{0}\"")]
        String,

        [PropertyTypeMapping(typeof(bool))]
        Bool,

        [PropertyTypeMapping(typeof(byte))]
        Byte,

        [PropertyTypeMapping(typeof(sbyte))]
        SByte,

        [PropertyTypeMapping(typeof(char), "'{0}'")]
        Char,

        [PropertyTypeMapping(typeof(decimal), typeof(DecimalValueHandler))]
        Decimal,

        [PropertyTypeMapping(typeof(double))]
        Double,

        [PropertyTypeMapping(typeof(float), typeof(FloatValueHandler))]
        Float,

        [PropertyTypeMapping(typeof(int))]
        Int,

        [PropertyTypeMapping(typeof(uint))]
        UInt,

        [PropertyTypeMapping(typeof(long))]
        Long,

        [PropertyTypeMapping(typeof(ulong))]
        ULong,

        [PropertyTypeMapping(typeof(short))]
        Short,

        [PropertyTypeMapping(typeof(ushort))]
        UShort,

        [PropertyTypeMapping(typeof(DateTime), "System.DateTime.Parse(\"{0}\")")]
        DateTime,

        [PropertyTypeMapping(typeof(DateTimeOffset), "System.DateTimeOffset.Parse(\"{0}\")")]
        DateTimeOffset,

        [PropertyTypeMapping(typeof(Guid), "new System.Guid(\"{0}\")")]
        Guid,

        [PropertyTypeMapping(typeof(Uri), typeof(UriValueHandler))]
        Uri,

        [PropertyTypeMapping(typeof(TimeSpan), "System.TimeSpan.Parse(\"{0}\")")]
        TimeSpan
    }
}
