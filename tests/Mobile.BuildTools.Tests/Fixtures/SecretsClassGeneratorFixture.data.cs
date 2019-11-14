using System;
using System.Collections.Generic;
using Mobile.BuildTools.Models.Secrets;

namespace Mobile.BuildTools.Tests.Fixtures
{
    public partial class SecretsClassGeneratorFixture
    {
        public static IEnumerable<object[]> GetSecretsFromDataGenerator()
        {
            // bool
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = false,
                    Type = PropertyType.Bool,
                    TypeDeclaration = "const bool",
                    Value = "False",
                    ValueDeclaration = "false"
                }
            };
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = true,
                    IsArray = false,
                    Type = PropertyType.Bool,
                    TypeDeclaration = "const bool",
                    Value = "False",
                    ValueDeclaration = "false"
                }
            };
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = true,
                    Type = PropertyType.Bool,
                    TypeDeclaration = "const bool[]",
                    Value = "False",
                    ValueDeclaration = "new[] { false, false }"
                }
            };
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = true,
                    IsArray = true,
                    Type = PropertyType.Bool,
                    TypeDeclaration = "const bool[]",
                    Value = "False",
                    ValueDeclaration = "new[] { false, false }"
                }
            };

            // byte
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = false,
                    Type = PropertyType.Byte,
                    TypeDeclaration = "const byte",
                    Value = "0",
                    ValueDeclaration = "0"
                }
            };
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = true,
                    Type = PropertyType.Byte,
                    TypeDeclaration = "const byte[]",
                    Value = "0;1",
                    ValueDeclaration = "new[] { 0, 1 }"
                }
            };

            // char
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = false,
                    Type = PropertyType.Char,
                    TypeDeclaration = "const char",
                    Value = "c",
                    ValueDeclaration = "'c'"
                }
            };
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = true,
                    Type = PropertyType.Bool,
                    TypeDeclaration = "const char[]",
                    Value = "c;d",
                    ValueDeclaration = "new[] { 'c', 'd' }"
                }
            };

            // DateTime
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = false,
                    Type = PropertyType.DateTime,
                    TypeDeclaration = "static readonly System.DateTime",
                    Value = "July 11, 2019 08:00",
                    ValueDeclaration = "System.DateTime.Parse(\"July 11, 2019 08:00\")"
                }
            };
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = true,
                    Type = PropertyType.DateTime,
                    TypeDeclaration = "static readonly DateTime[]",
                    Value = "July 11, 2019 08:00;July 13, 2019 17:00",
                    ValueDeclaration = "new[] { System.DateTime.Parse(\"July 11, 2019 08:00\"), System.DateTime.Parse(\"July 13, 2019 17:00\") }"
                }
            };

            // DateTimeOffset
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = false,
                    Type = PropertyType.DateTimeOffset,
                    TypeDeclaration = "static readonly System.DateTimeOffset",
                    Value = "July 11, 2019 08:00;July 13, 2019 17:00",
                    ValueDeclaration = "System.DateTimeOffset.Parse(\"July 11, 2019 08:00\")"
                }
            };
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = true,
                    Type = PropertyType.Bool,
                    TypeDeclaration = "static readonly System.DateTimeOffset[]",
                    Value = "July 11, 2019 08:00 -7;July 13, 2019 17:00 -7",
                    ValueDeclaration = "new[] { System.DateTimeOffset.Parse(\"July 11, 2019 08:00 -7\"), System.DateTimeOffset.Parse(\"July 13, 2019 17:00 -7\") }"
                }
            };

            // Decimal
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = false,
                    Type = PropertyType.Decimal,
                    TypeDeclaration = "const decimal",
                    Value = "5.3M",
                    ValueDeclaration = "5.3M"
                }
            };
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = true,
                    Type = PropertyType.Decimal,
                    TypeDeclaration = "const decimal[]",
                    Value = "5.3M;3.3M",
                    ValueDeclaration = "new[] { 5.3M, 3.3M }"
                }
            };

            // Double
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = false,
                    Type = PropertyType.Double,
                    TypeDeclaration = "const double",
                    Value = "5.2",
                    ValueDeclaration = "5.2"
                }
            };
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = true,
                    IsArray = false,
                    Type = PropertyType.Double,
                    TypeDeclaration = "const double",
                    Value = "5.2",
                    ValueDeclaration = "5.2"
                }
            };
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = true,
                    Type = PropertyType.Double,
                    TypeDeclaration = "const double[]",
                    Value = "5.3;3.9",
                    ValueDeclaration = "new[] { 5.3, 3.9 }"
                }
            };
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = true,
                    IsArray = true,
                    Type = PropertyType.Double,
                    TypeDeclaration = "const double[]",
                    Value = "5.3;3.9",
                    ValueDeclaration = "new[] { 5.3, 3.9 }"
                }
            };

            // Float
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = false,
                    Type = PropertyType.Float,
                    TypeDeclaration = "const float",
                    Value = "5.3F",
                    ValueDeclaration = "5.3F"
                }
            };
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = true,
                    Type = PropertyType.Float,
                    TypeDeclaration = "const float[]",
                    Value = "5.3F;3.9F",
                    ValueDeclaration = "new[] { 5.3F, 3.9F }"
                }
            };

            // Guid
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = false,
                    Type = PropertyType.Guid,
                    TypeDeclaration = "static readonly System.Guid",
                    Value = "0d1d64bf-66a7-4455-a94e-459aabf84cca",
                    ValueDeclaration = "new System.Guid(\"0d1d64bf-66a7-4455-a94e-459aabf84cca\")"
                }
            };
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = true,
                    Type = PropertyType.Guid,
                    TypeDeclaration = "const Guid[]",
                    Value = "0d1d64bf-66a7-4455-a94e-459aabf84cca;f476d60d-1c71-4771-a0ce-d72ac7f3cd35",
                    ValueDeclaration = "new[] { new System.Guid(\"0d1d64bf-66a7-4455-a94e-459aabf84cca\"), new System.Guid(\"f476d60d-1c71-4771-a0ce-d72ac7f3cd35\") }"
                }
            };

            // Int
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = false,
                    Type = PropertyType.Int,
                    TypeDeclaration = "const int",
                    Value = "5",
                    ValueDeclaration = "5"
                }
            };
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = true,
                    IsArray = false,
                    Type = PropertyType.Int,
                    TypeDeclaration = "const int",
                    Value = "6",
                    ValueDeclaration = "6"
                }
            };
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = true,
                    Type = PropertyType.Int,
                    TypeDeclaration = "const int[]",
                    Value = "5;6",
                    ValueDeclaration = "new[] { 5, 6 }"
                }
            };
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = true,
                    IsArray = true,
                    Type = PropertyType.Int,
                    TypeDeclaration = "const int[]",
                    Value = "5;6",
                    ValueDeclaration = "new[] { 5, 6 }"
                }
            };

            // Long
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = false,
                    Type = PropertyType.Long,
                    TypeDeclaration = "const long",
                    Value = "5",
                    ValueDeclaration = "5"
                }
            };
            yield return new object[]
            {
                new SecretValue
                {
                    FirstRun = false,
                    IsArray = true,
                    Type = PropertyType.Long,
                    TypeDeclaration = "const long[]",
                    Value = "5;6",
                    ValueDeclaration = "new[] { 5, 6 }"
                }
            };


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
    }
}
