using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators.Secrets;
using Mobile.BuildTools.Models.Secrets;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures.Generators
{
    public partial class SecretsClassGeneratorFixture : FixtureBase
    {
        public const string TestKey = nameof(TestKey);

        public SecretsClassGeneratorFixture(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        private SecretsClassGenerator GetGenerator(string testName = null)
        {
            if(string.IsNullOrEmpty(testName))
            {
                var st = new StackTrace();
                testName = st.GetFrame(1).GetMethod().Name;
            }
            return GetGenerator(GetConfiguration(testName));
        }

        private SecretsClassGenerator GetGenerator(IBuildConfiguration config) =>
            new SecretsClassGenerator(config);

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ProcessSecretHidesValueForSafeOutput(bool firstRun)
        {
            var value = "TestValue";
            var pair = new KeyValuePair<string, JToken>(TestKey, value);
            var generator = GetGenerator();
            var config = new SecretsConfig()
            {
                Properties = new List<ValueConfig>{ new ValueConfig { Name = TestKey, PropertyType = PropertyType.String } }
            };
            var output = generator.ProcessSecret(pair, config, firstRun, true);
            Assert.DoesNotContain(value, output);
            Assert.Contains("*****", output);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ProcessSecretGeneratesBooleanValue(bool firstRun)
        {
            var key = "TestKey";
            var value = true;
            var pair = new KeyValuePair<string, JToken>(key, value);
            var generator = GetGenerator();
            var config = new SecretsConfig()
            {
                Properties = new List<ValueConfig> { new ValueConfig { Name = TestKey, PropertyType = PropertyType.Bool } }
            };

            var output = generator.ProcessSecret(pair, config, firstRun);
            Assert.Contains($"const bool {key}", output);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ProcessSecretGeneratesIntegerValue(bool firstRun)
        {
            var key = "TestKey";
            var value = 2;
            var pair = new KeyValuePair<string, JToken>(key, value);
            var generator = GetGenerator();
            var config = new SecretsConfig()
            {
                Properties = new List<ValueConfig> { new ValueConfig { Name = TestKey, PropertyType = PropertyType.Int } }
            };

            var output = generator.ProcessSecret(pair, config, firstRun);
            Assert.Contains($"const int {key}", output);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ProcessSecretGeneratesDoubleValue(bool firstRun)
        {
            var key = "TestKey";
            var value = 2.2;
            var pair = new KeyValuePair<string, JToken>(key, value);
            var generator = GetGenerator();
            var config = new SecretsConfig()
            {
                Properties = new List<ValueConfig> { new ValueConfig { Name = TestKey, PropertyType = PropertyType.Double } }
            };

            var output = generator.ProcessSecret(pair, config, firstRun);
            Assert.Contains($"const double {key}", output);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ProcessSecretGeneratesStringValue(bool firstRun)
        {
            var key = "TestKey";
            var value = "TestValue";
            var pair = new KeyValuePair<string, JToken>(key, value);
            var generator = GetGenerator();
            var config = new SecretsConfig()
            {
                Properties = new List<ValueConfig> { new ValueConfig { Name = TestKey, PropertyType = PropertyType.String } }
            };

            var output = generator.ProcessSecret(pair, config, firstRun);
            Assert.Contains($"const string {key}", output);
        }

        [Theory]
        [InlineData("Helpers", "Foo.Bar.Helpers")]
        [InlineData(".Helpers", "Foo.Bar.Helpers")]
        [InlineData(".", "Foo.Bar")]
        [InlineData("Helpers.Configuration", "Foo.Bar.Helpers.Configuration")]
        [InlineData("Helpers/Configuration", "Foo.Bar.Helpers.Configuration")]
        [InlineData("/Helpers/Configuration", "Foo.Bar.Helpers.Configuration")]
        public void GeneratesCorrectNamespace_WithSpecifiedBaseNamespace(string relativeNamespaceConfig, string expectedNamespace)
        {
            var buildConfig = GetConfiguration();
            var generator = GetGenerator(buildConfig);
            generator.BaseNamespace = "Foo.Bar";
            buildConfig.ProjectDirectory = "file://Repos/AwesomeProject/Foo";

            string @namespace = null;
            var exception = Record.Exception(() => @namespace = generator.GetNamespace(relativeNamespaceConfig));
            Assert.Null(exception);

            Assert.Equal(expectedNamespace, @namespace);
        }

        [Theory]
        // bool
        [InlineData(false, false, PropertyType.Bool, "const bool", "False", "false")]
        [InlineData(false, true, PropertyType.Bool, "static readonly bool[]", "False;True", "new bool[] { false, true }")]
        [InlineData(true, false, PropertyType.Bool, "const bool", "False", "false")]
        [InlineData(true, true, PropertyType.Bool, "static readonly bool[]", "False;True", "new bool[] { false, true }")]
        // byte
        [InlineData(false, false, PropertyType.Byte, "const byte", "0", "0")]
        [InlineData(false, true, PropertyType.Byte, "static readonly byte[]", "0;1", "new byte[] { 0, 1 }")]
        // char
        [InlineData(false, false, PropertyType.Char, "const char", "c", "c")]
        [InlineData(false, true, PropertyType.Char, "static readonly char[]", "h;e;l;l;o", "new char[] { 'h', 'e', 'l', 'l', 'o' }")]
        // DateTime
        [InlineData(false, false, PropertyType.DateTime, "static readonly System.DateTime", "July 11, 2019 08:00", "System.DateTime.Parse(\"July 11, 2019 08:00\")")]
        [InlineData(false, true, PropertyType.DateTime, "static readonly System.DateTime[]", "July 11, 2019 08:00;July 13, 2019 17:00", "new System.DateTime[] { System.DateTime.Parse(\"July 11, 2019 08:00\"), System.DateTime.Parse(\"July 13, 2019 17:00\") }")]
        // DateTimeOffset
        [InlineData(false, false, PropertyType.DateTimeOffset, "static readonly System.DateTimeOffset", "July 11, 2019 08:00", "System.DateTimeOffset.Parse(\"July 11, 2019 08:00\")")]
        [InlineData(false, true, PropertyType.DateTimeOffset, "static readonly System.DateTimeOffset[]", "July 11, 2019 08:00 -7;July 13, 2019 17:00 -7", "new System.DateTimeOffset[] { System.DateTimeOffset.Parse(\"July 11, 2019 08:00 -7\"), System.DateTimeOffset.Parse(\"July 13, 2019 17:00 -7\") }")]
        // Decimal
        [InlineData(false, false, PropertyType.Decimal, "const decimal", "5.3M", "5.3M")]
        [InlineData(false, true, PropertyType.Decimal, "static readonly decimal[]", "5.3M;3.3M", "new decimal[] { 5.3M, 3.3M }")]
        // Double
        [InlineData(false, false, PropertyType.Double, "const double", "5.3", "5.3")]
        [InlineData(true, false, PropertyType.Double, "const double", "5.3", "5.3")]
        [InlineData(false, true, PropertyType.Double, "static readonly double[]", "5.3;3.3", "new double[] { 5.3, 3.3 }")]
        [InlineData(true, true, PropertyType.Double, "static readonly double[]", "5.3;3.3", "new double[] { 5.3, 3.3 }")]
        // Float
        [InlineData(false, false, PropertyType.Float, "const float", "5.3F", "5.3F")]
        [InlineData(false, true, PropertyType.Float, "static readonly float[]", "5.3F;3.3F", "new float[] { 5.3F, 3.3F }")]
        // Guid
        [InlineData(false, false, PropertyType.Guid, "static readonly System.Guid", "0d1d64bf-66a7-4455-a94e-459aabf84cca", "new System.Guid(\"0d1d64bf-66a7-4455-a94e-459aabf84cca\")")]
        [InlineData(false, true, PropertyType.Guid, "static readonly System.Guid[]", "0d1d64bf-66a7-4455-a94e-459aabf84cca;f476d60d-1c71-4771-a0ce-d72ac7f3cd35", "new System.Guid[] { new System.Guid(\"0d1d64bf-66a7-4455-a94e-459aabf84cca\"), new System.Guid(\"f476d60d-1c71-4771-a0ce-d72ac7f3cd35\") }")]
        // Int
        [InlineData(false, false, PropertyType.Int, "const int", "6", "6")]
        [InlineData(false, false, PropertyType.Int, "const int", "6.0", "6")]
        [InlineData(true, false, PropertyType.Int, "const int", "6", "6")]
        [InlineData(true, false, PropertyType.Int, "const int", "6.0", "6")]
        [InlineData(false, true, PropertyType.Int, "static readonly int[]", "6;5", "new int[] { 6, 5 }")]
        [InlineData(true, true, PropertyType.Int, "static readonly int[]", "6;5", "new int[] { 6, 5 }")]
        // Long
        [InlineData(false, false, PropertyType.Long, "const long", "5", "5")]
        [InlineData(false, true, PropertyType.Long, "static readonly long[]", "5;6", "new long[] { 5, 6 }")]
        // SByte
        [InlineData(false, false, PropertyType.SByte, "const sbyte", "3", "3")]
        [InlineData(false, true, PropertyType.SByte, "static readonly sbyte[]", "2;3", "new sbyte[] { 2, 3 }")]
        // Short
        [InlineData(false, false, PropertyType.Short, "const short", "2", "2")]
        [InlineData(false, true, PropertyType.Short, "static readonly short[]", "2;3", "new short[] { 2, 3 }")]
        // String
        [InlineData(false, false, PropertyType.String, "const string", "Hello World", "Hello World")]
        [InlineData(false, true, PropertyType.String, "static readonly string[]", "Hello;World", "new string[] { \"Hello\", \"World\" }")]
        [InlineData(true, false, PropertyType.String, "const string", "Hello World", "Hello World")]
        [InlineData(true, true, PropertyType.String, "static readonly string[]", "Hello;World", "new string[] { \"Hello\", \"World\" }")]
        // TimeSpan
        [InlineData(false, false, PropertyType.TimeSpan, "static readonly System.TimeSpan", "6:12", "System.TimeSpan.Parse(\"6:12\")")]
        [InlineData(false, true, PropertyType.TimeSpan, "static readonly System.TimeSpan[]", "6:12;4:20", "new System.TimeSpan[] { System.TimeSpan.Parse(\"6:12\"), System.TimeSpan.Parse(\"4:20\") }")]
        public void ProcessSecretValue(bool firstRun, bool isArray, PropertyType type, string typeDeclaration, string value, string valueDeclaration)
        {
            var pair = new KeyValuePair<string, JToken>(TestKey, value);
            var generator = GetGenerator();
            var valueConfig = new ValueConfig
            {
                Name = TestKey,
                PropertyType = type,
                IsArray = isArray
            };
            var secretsConfig = new SecretsConfig { Properties = new List<ValueConfig> { valueConfig } };
            var output = generator.ProcessSecret(pair, secretsConfig, firstRun);
            Assert.Contains($"{typeDeclaration} {TestKey}", output);
            Assert.Contains(valueDeclaration, output);
        }

        [Theory]
        [InlineData("bool.json", PropertyType.Bool, typeof(bool), false)]
        [InlineData("boolArray.json", PropertyType.Bool, typeof(bool[]), true)]
        [InlineData("byte.json", PropertyType.Byte, typeof(byte), false)]
        [InlineData("byteArray.json", PropertyType.Byte, typeof(byte[]), true)]
        [InlineData("char.json", PropertyType.Char, typeof(char), false)]
        [InlineData("charArray.json", PropertyType.Char, typeof(char[]), true)]
        [InlineData("datetime.json", PropertyType.DateTime, typeof(DateTime), false)]
        [InlineData("datetimeArray.json", PropertyType.DateTime, typeof(DateTime[]), true)]
        [InlineData("datetimeoffset.json", PropertyType.DateTimeOffset, typeof(DateTimeOffset), false)]
        [InlineData("datetimeoffsetArray.json", PropertyType.DateTimeOffset, typeof(DateTimeOffset[]), true)]
        [InlineData("decimal.json", PropertyType.Decimal, typeof(decimal), false)]
        [InlineData("decimalArray.json", PropertyType.Decimal, typeof(decimal[]), true)]
        [InlineData("double.json", PropertyType.Double, typeof(double), false)]
        [InlineData("doubleArray.json", PropertyType.Double, typeof(double[]), true)]
        [InlineData("float.json", PropertyType.Float, typeof(float), false)]
        [InlineData("floatArray.json", PropertyType.Float, typeof(float[]), true)]
        [InlineData("guid.json", PropertyType.Guid, typeof(Guid), false)]
        [InlineData("guidArray.json", PropertyType.Guid, typeof(Guid[]), true)]
        [InlineData("int.json", PropertyType.Int, typeof(int), false)]
        [InlineData("intArray.json", PropertyType.Int, typeof(int[]), true)]
        [InlineData("long.json", PropertyType.Long, typeof(long), false)]
        [InlineData("longArray.json", PropertyType.Long, typeof(long[]), true)]
        [InlineData("sbyte.json", PropertyType.SByte, typeof(sbyte), false)]
        [InlineData("sbyteArray.json", PropertyType.SByte, typeof(sbyte[]), true)]
        [InlineData("short.json", PropertyType.Short, typeof(short), false)]
        [InlineData("shortArray.json", PropertyType.Short, typeof(short[]), true)]
        [InlineData("string.json", PropertyType.String, typeof(string), false)]
        [InlineData("stringArray.json", PropertyType.String, typeof(string[]), true)]
        [InlineData("timespan.json", PropertyType.TimeSpan, typeof(TimeSpan), false)]
        [InlineData("timespanArray.json", PropertyType.TimeSpan, typeof(TimeSpan[]), true)]
        [InlineData("uint.json", PropertyType.UInt, typeof(uint), false)]
        [InlineData("uintArray.json", PropertyType.UInt, typeof(uint[]), true)]
        [InlineData("ulong.json", PropertyType.ULong, typeof(ulong), false)]
        [InlineData("ulongArray.json", PropertyType.ULong, typeof(ulong[]), true)]
        [InlineData("uri.json", PropertyType.Uri, typeof(Uri), false)]
        [InlineData("uriArray.json", PropertyType.Uri, typeof(Uri[]), true)]
        [InlineData("ushort.json", PropertyType.UShort, typeof(ushort), false)]
        [InlineData("ushortArray.json", PropertyType.UShort, typeof(ushort[]), true)]
        public void GeneratesValidClass(string secretsFile, PropertyType propertyType, Type expectedType, bool isArray)
        {
            var config = GetConfiguration($"{nameof(GeneratesValidClass)}-{expectedType.Name}");
            config.SecretsConfig.Properties.Add(new ValueConfig
            {
                Name = "Prop",
                PropertyType = propertyType,
                IsArray = isArray
            });
            var generator = new SecretsClassGenerator(config)
            {
                SecretsJsonFilePath = Path.Combine("Templates", "Secrets", secretsFile),
                BaseNamespace = config.ProjectName
            };
            generator.Execute();

            var filePath = generator.Outputs.ItemSpec;
            Assert.True(File.Exists(filePath));

            var csc = new CSharpCodeProvider();
            var parameters = new CompilerParameters(new[] { "mscorlib.dll", "System.dll", "System.Core.dll" }, Path.Combine(config.IntermediateOutputPath, $"{Path.GetFileNameWithoutExtension(secretsFile)}.dll"), true);
            var results = csc.CompileAssemblyFromFile(parameters, filePath);
            results.Errors.Cast<CompilerError>().ToList().ForEach(error => _testOutputHelper.WriteLine(error.ErrorText));

            Assert.Empty(results.Errors);
            Assert.NotNull(results.CompiledAssembly);
            var secretsClassType = results.CompiledAssembly.DefinedTypes.FirstOrDefault(t => t.Name == config.SecretsConfig.ClassName);
            Assert.NotNull(secretsClassType);
            var propField = secretsClassType.GetField("Prop", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(propField);
            Assert.Equal(expectedType.FullName, propField.FieldType.FullName);

            object value = null;
            var ex = Record.Exception(() => value = propField.GetValue(null));
            Assert.Null(ex);
            Assert.NotNull(value);

            if(isArray)
            {
                var array = (IEnumerable)value;
                Assert.NotNull(array);
                Assert.Equal(3, array.Cast<object>().Count());
            }
        }

        public class SecretValue
        {
            public PropertyType Type { get; set; }
            public string Value { get; set; }
            public bool FirstRun { get; set; }
            public bool IsArray { get; set; }
            public string TypeDeclaration { get; set; }
            public string ValueDeclaration { get; set; }
        }
    }
}
