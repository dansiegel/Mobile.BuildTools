using System.Collections;
using System.Collections.Generic;
using Mobile.BuildTools.Models.Settings;
using static Mobile.BuildTools.Tests.Fixtures.Generators.SecretsClassGeneratorFixture;

namespace Mobile.BuildTools.Tests.Data
{
    internal class SecretsData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            //// bool
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = false,
            //        Type = PropertyType.Bool,
            //        TypeDeclaration = "const bool",
            //        Value = "False",
            //        ValueDeclaration = "false"
            //    }
            //};
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = true,
            //        IsArray = false,
            //        Type = PropertyType.Bool,
            //        TypeDeclaration = "const bool",
            //        Value = "False",
            //        ValueDeclaration = "false"
            //    }
            //};
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = true,
            //        Type = PropertyType.Bool,
            //        TypeDeclaration = "const bool[]",
            //        Value = "False",
            //        ValueDeclaration = "new[] { false, false }"
            //    }
            //};
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = true,
            //        IsArray = true,
            //        Type = PropertyType.Bool,
            //        TypeDeclaration = "const bool[]",
            //        Value = "False",
            //        ValueDeclaration = "new[] { false, false }"
            //    }
            //};

            //// byte
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = false,
            //        Type = PropertyType.Byte,
            //        TypeDeclaration = "const byte",
            //        Value = "0",
            //        ValueDeclaration = "0"
            //    }
            //};
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = true,
            //        Type = PropertyType.Byte,
            //        TypeDeclaration = "const byte[]",
            //        Value = "0;1",
            //        ValueDeclaration = "new[] { 0, 1 }"
            //    }
            //};

            //// char
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = false,
            //        Type = PropertyType.Char,
            //        TypeDeclaration = "const char",
            //        Value = "c",
            //        ValueDeclaration = "'c'"
            //    }
            //};
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = true,
            //        Type = PropertyType.Bool,
            //        TypeDeclaration = "const char[]",
            //        Value = "c;d",
            //        ValueDeclaration = "new[] { 'c', 'd' }"
            //    }
            //};

            //// DateTime
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = false,
            //        Type = PropertyType.DateTime,
            //        TypeDeclaration = "static readonly System.DateTime",
            //        Value = ,
            //        ValueDeclaration = 
            //    }
            //};
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = true,
            //        Type = PropertyType.DateTime,
            //        TypeDeclaration = ,
            //        Value = ,
            //        ValueDeclaration = 
            //    }
            //};

            //// DateTimeOffset
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = false,
            //        Type = PropertyType.DateTimeOffset,
            //        TypeDeclaration = ,
            //        Value = ,
            //        ValueDeclaration = 
            //    }
            //};
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = true,
            //        Type = PropertyType.DateTimeOffset,
            //        TypeDeclaration = ,
            //        Value = ,
            //        ValueDeclaration = 
            //    }
            //};

            // Decimal
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = false,
            //        Type = PropertyType.Decimal,
            //        TypeDeclaration = "const decimal",
            //        Value = "5.3M",
            //        ValueDeclaration = "5.3M"
            //    }
            //};
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = true,
            //        Type = PropertyType.Decimal,
            //        TypeDeclaration = "const decimal[]",
            //        Value = "5.3M;3.3M",
            //        ValueDeclaration = "new[] { 5.3M, 3.3M }"
            //    }
            //};

            // Double
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = false,
            //        Type = PropertyType.Double,
            //        TypeDeclaration = "const double",
            //        Value = "5.2",
            //        ValueDeclaration = "5.2"
            //    }
            //};
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = true,
            //        IsArray = false,
            //        Type = PropertyType.Double,
            //        TypeDeclaration = "const double",
            //        Value = "5.2",
            //        ValueDeclaration = "5.2"
            //    }
            //};
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = true,
            //        Type = PropertyType.Double,
            //        TypeDeclaration = "const double[]",
            //        Value = "5.3;3.9",
            //        ValueDeclaration = "new[] { 5.3, 3.9 }"
            //    }
            //};
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = true,
            //        IsArray = true,
            //        Type = PropertyType.Double,
            //        TypeDeclaration = "const double[]",
            //        Value = "5.3;3.9",
            //        ValueDeclaration = "new[] { 5.3, 3.9 }"
            //    }
            //};

            // Float
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = false,
            //        Type = PropertyType.Float,
            //        TypeDeclaration = "const float",
            //        Value = "5.3F",
            //        ValueDeclaration = "5.3F"
            //    }
            //};
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = true,
            //        Type = PropertyType.Float,
            //        TypeDeclaration = "const float[]",
            //        Value = "5.3F;3.9F",
            //        ValueDeclaration = "new[] { 5.3F, 3.9F }"
            //    }
            //};

            // Guid
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = false,
            //        Type = PropertyType.Guid,
            //        TypeDeclaration = ,
            //        Value = ,
            //        ValueDeclaration = 
            //    }
            //};
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = true,
            //        Type = PropertyType.Guid,
            //        TypeDeclaration = ,
            //        Value = ,
            //        ValueDeclaration = 
            //    }
            //};

            // Int
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = false,
            //        Type = PropertyType.Int,
            //        TypeDeclaration = "const int",
            //        Value = "5",
            //        ValueDeclaration = "5"
            //    }
            //};
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = true,
            //        IsArray = false,
            //        Type = PropertyType.Int,
            //        TypeDeclaration = "const int",
            //        Value = "6",
            //        ValueDeclaration = "6"
            //    }
            //};
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = true,
            //        Type = PropertyType.Int,
            //        TypeDeclaration = "const int[]",
            //        Value = "5;6",
            //        ValueDeclaration = "new[] { 5, 6 }"
            //    }
            //};
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = true,
            //        IsArray = true,
            //        Type = PropertyType.Int,
            //        TypeDeclaration = "const int[]",
            //        Value = "5;6",
            //        ValueDeclaration = "new[] { 5, 6 }"
            //    }
            //};

            // Long
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = false,
            //        Type = PropertyType.Long,
            //        TypeDeclaration = "const long",
            //        Value = "5",
            //        ValueDeclaration = "5"
            //    }
            //};
            //yield return new object[]
            //{
            //    new SecretValue
            //    {
            //        FirstRun = false,
            //        IsArray = true,
            //        Type = PropertyType.Long,
            //        TypeDeclaration = "const long[]",
            //        Value = "5;6",
            //        ValueDeclaration = "new[] { 5, 6 }"
            //    }
            //};


            // SByte
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = false,
                    Type = PropertyType.SByte,
                    TypeDeclaration = "const sbyte",
                    Value = "3",
                    ValueDeclaration = "3"
                }
            };
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = true,
                    Type = PropertyType.SByte,
                    TypeDeclaration = "const sbyte[]",
                    Value = "1;3",
                    ValueDeclaration = "new[] { 1, 3 }"
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
