using System;

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

        [PropertyTypeMapping(typeof(decimal))]
        Decimal,

        [PropertyTypeMapping(typeof(double))]
        Double,

        [PropertyTypeMapping(typeof(float))]
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

        [PropertyTypeMapping(typeof(Uri), "new System.Uri(\"{0}\")")]
        Uri,

        [PropertyTypeMapping(typeof(TimeSpan), "System.TimeSpan.Parse(\"{0}\")")]
        TimeSpan
    }
}
