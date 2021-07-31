﻿using System;
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
using Mobile.BuildTools.Models.Settings;
using Newtonsoft.Json;
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
            var pair = new KeyValuePair<string, string>(TestKey, value);
            var generator = GetGenerator();
            var config = new SettingsConfig()
            {
                Properties = new List<ValueConfig>{ new ValueConfig { Name = TestKey, PropertyType = PropertyType.String } }
            };
            var output = generator.ProcessSecret(pair, config, firstRun, true);
            Assert.DoesNotContain(value, output);
            Assert.Contains("*****", output);
        }

        [Theory]
        [InlineData(Accessibility.Internal, true)]
        [InlineData(Accessibility.Internal, false)]
        [InlineData(Accessibility.Public, true)]
        [InlineData(Accessibility.Public, false)]
        public void ProcessSecretGeneratesBooleanValue(Accessibility accessibility, bool firstRun)
        {
            var key = "TestKey";
            var value = bool.TrueString;
            var pair = new KeyValuePair<string, string>(key, value);
            var generator = GetGenerator();
            var config = new SettingsConfig()
            {
                Accessibility = accessibility,
                Properties = new List<ValueConfig> { new ValueConfig { Name = TestKey, PropertyType = PropertyType.Bool } }
            };

            var output = generator.ProcessSecret(pair, config, firstRun);
            Assert.Contains($"{accessibility.ToString().ToLower()} const bool {key}", output);
        }

        [Theory]
        [InlineData(Accessibility.Internal, true)]
        [InlineData(Accessibility.Internal, false)]
        [InlineData(Accessibility.Public, true)]
        [InlineData(Accessibility.Public, false)]
        public void ProcessSecretGeneratesIntegerValue(Accessibility accessibility, bool firstRun)
        {
            var key = "TestKey";
            var value = "2";
            var pair = new KeyValuePair<string, string>(key, value);
            var generator = GetGenerator();
            var config = new SettingsConfig()
            {
                Accessibility = accessibility,
                Properties = new List<ValueConfig> { new ValueConfig { Name = TestKey, PropertyType = PropertyType.Int } }
            };

            var output = generator.ProcessSecret(pair, config, firstRun);
            Assert.Contains($"{accessibility.ToString().ToLower()} const int {key}", output);
        }

        [Theory]
        [InlineData(Accessibility.Internal, true)]
        [InlineData(Accessibility.Internal, false)]
        [InlineData(Accessibility.Public, true)]
        [InlineData(Accessibility.Public, false)]
        public void ProcessSecretGeneratesDoubleValue(Accessibility accessibility, bool firstRun)
        {
            var key = "TestKey";
            var value = "2.2";
            var pair = new KeyValuePair<string, string>(key, value);
            var generator = GetGenerator();
            var config = new SettingsConfig()
            {
                Accessibility = accessibility,
                Properties = new List<ValueConfig> { new ValueConfig { Name = TestKey, PropertyType = PropertyType.Double } }
            };

            var output = generator.ProcessSecret(pair, config, firstRun);
            Assert.Contains($"{accessibility.ToString().ToLower()} const double {key}", output);
        }

        [Theory]
        [InlineData(Accessibility.Internal, true)]
        [InlineData(Accessibility.Internal, false)]
        [InlineData(Accessibility.Public, true)]
        [InlineData(Accessibility.Public, false)]
        public void ProcessSecretGeneratesStringValue(Accessibility accessibility, bool firstRun)
        {
            var key = "TestKey";
            var value = "TestValue";
            var pair = new KeyValuePair<string, string>(key, value);
            var generator = GetGenerator();
            var config = new SettingsConfig()
            {
                Accessibility = accessibility,
                Properties = new List<ValueConfig> { new ValueConfig { Name = TestKey, PropertyType = PropertyType.String } }
            };

            var output = generator.ProcessSecret(pair, config, firstRun);
            Assert.Contains($"{accessibility.ToString().ToLower()} const string {key}", output);
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
            generator.RootNamespace = "Foo.Bar";
            buildConfig.ProjectDirectory = "file://Repos/AwesomeProject/Foo";

            string @namespace = null;
            var settingsConfig = new SettingsConfig
            {
                Namespace = relativeNamespaceConfig
            };
            var exception = Record.Exception(() => @namespace = generator.GetNamespace(settingsConfig));
            Assert.Null(exception);

            Assert.Equal(expectedNamespace, @namespace);
        }

        [Theory]
        [InlineData(false, PropertyType.Bool, "const bool", "false")]
        [InlineData(true, PropertyType.Bool, "static readonly bool[]", "System.Array.Empty<bool>()")]
        [InlineData(false, PropertyType.Byte, "const byte", "default")]
        [InlineData(true, PropertyType.Byte, "static readonly byte[]", "System.Array.Empty<byte>()")]
        [InlineData(false, PropertyType.Char, "const char", "default")]
        [InlineData(true, PropertyType.Char, "static readonly char[]", "System.Array.Empty<char>()")]
        [InlineData(false, PropertyType.DateTime, "static readonly System.DateTime", "default")]
        [InlineData(true, PropertyType.DateTime, "static readonly System.DateTime[]", "System.Array.Empty<System.DateTime>()")]
        [InlineData(false, PropertyType.DateTimeOffset, "static readonly System.DateTimeOffset", "default")]
        [InlineData(true, PropertyType.DateTimeOffset, "static readonly System.DateTimeOffset[]", "System.Array.Empty<System.DateTimeOffset>()")]
        [InlineData(false, PropertyType.Decimal, "const decimal", "default")]
        [InlineData(true, PropertyType.Decimal, "static readonly decimal[]", "System.Array.Empty<decimal>()")]
        [InlineData(false, PropertyType.Double, "const double", "default")]
        [InlineData(true, PropertyType.Double, "static readonly double[]", "System.Array.Empty<double>()")]
        [InlineData(false, PropertyType.Float, "const float", "default")]
        [InlineData(true, PropertyType.Float, "static readonly float[]", "System.Array.Empty<float>()")]
        [InlineData(false, PropertyType.Guid, "static readonly System.Guid", "default")]
        [InlineData(true, PropertyType.Guid, "static readonly System.Guid[]", "System.Array.Empty<System.Guid>()")]
        [InlineData(false, PropertyType.Int, "const int", "default")]
        [InlineData(true, PropertyType.Int, "static readonly int[]", "System.Array.Empty<int>()")]
        [InlineData(false, PropertyType.Long, "const long", "default")]
        [InlineData(true, PropertyType.Long, "static readonly long[]", "System.Array.Empty<long>()")]
        [InlineData(false, PropertyType.SByte, "const sbyte", "default")]
        [InlineData(true, PropertyType.SByte, "static readonly sbyte[]", "System.Array.Empty<sbyte>()")]
        [InlineData(false, PropertyType.Short, "const short", "default")]
        [InlineData(true, PropertyType.Short, "static readonly short[]", "System.Array.Empty<short>()")]
        [InlineData(false, PropertyType.String, "const string", "default")]
        [InlineData(true, PropertyType.String, "static readonly string[]", "System.Array.Empty<string>()")]
        [InlineData(false, PropertyType.TimeSpan, "static readonly System.TimeSpan", "default")]
        [InlineData(true, PropertyType.TimeSpan, "static readonly System.TimeSpan[]", "System.Array.Empty<System.TimeSpan>()")]
        [InlineData(false, PropertyType.Uri, "static readonly System.Uri", "default")]
        [InlineData(true, PropertyType.Uri, "static readonly System.Uri[]", "System.Array.Empty<System.Uri>()")]
        public void HandleNullValue(bool isArray, PropertyType type, string typeDeclaration, string valueDeclaration)
        {
            var pair = new KeyValuePair<string, string>(TestKey, "null");
            var generator = GetGenerator();
            var valueConfig = new ValueConfig
            {
                Name = TestKey,
                PropertyType = type,
                IsArray = isArray
            };
            var secretsConfig = new SettingsConfig { Properties = new List<ValueConfig> { valueConfig } };
            var output = generator.ProcessSecret(pair, secretsConfig, false);
            Assert.Contains($"{typeDeclaration} {TestKey}", output);
            Assert.Contains(valueDeclaration, output);
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
            var pair = new KeyValuePair<string, string>(TestKey, value);
            var generator = GetGenerator();
            var valueConfig = new ValueConfig
            {
                Name = TestKey,
                PropertyType = type,
                IsArray = isArray
            };
            var secretsConfig = new SettingsConfig { Properties = new List<ValueConfig> { valueConfig } };
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
            var testDirectory = Path.Combine("Tests", nameof(SecretsClassGeneratorFixture), nameof(GeneratesValidClass), expectedType.Name);
            if (Directory.Exists(testDirectory))
                Directory.Delete(testDirectory, true);

            Directory.CreateDirectory(testDirectory);
            File.Copy(Path.Combine("Templates", "Secrets", secretsFile), Path.Combine(testDirectory, "appsettings.json"));

            var config = GetConfiguration($"{nameof(GeneratesValidClass)}-{expectedType.Name}");
            config.SolutionDirectory = config.ProjectDirectory = testDirectory;
            config.SettingsConfig = new List<SettingsConfig>{
                new SettingsConfig
                {
                    Properties = new List<ValueConfig>
                    {
                        new ValueConfig
                        {
                            Name = "Prop",
                            PropertyType = propertyType,
                            IsArray = isArray
                        }
                    }
                }
            };
            var generator = new SecretsClassGenerator(config)
            {
                RootNamespace = config.ProjectName
            };
            generator.Execute();

            Assert.Single(generator.Outputs);

            var filePath = generator.Outputs.First().ItemSpec;
            Assert.True(File.Exists(filePath));

            var csc = new CSharpCodeProvider();
            var parameters = new CompilerParameters(new[] { "mscorlib.dll", "System.dll", "System.Core.dll" }, Path.Combine(config.IntermediateOutputPath, $"{Path.GetFileNameWithoutExtension(secretsFile)}.dll"), true);
            var results = csc.CompileAssemblyFromFile(parameters, filePath);
            results.Errors.Cast<CompilerError>().ToList().ForEach(error => _testOutputHelper.WriteLine(error.ErrorText));

            Assert.Empty(results.Errors);
            Assert.NotNull(results.CompiledAssembly);
            var secretsClassType = results.CompiledAssembly.DefinedTypes.FirstOrDefault(t => t.Name == config.SettingsConfig.First().ClassName);
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

        [Fact]
        public void MergedSecretsContainsSingleElement()
        {
            var config = GetConfiguration();
            var settingsConfig = new SettingsConfig
            {
                Properties = new List<ValueConfig>
                {
                    new ValueConfig
                    {
                        Name = "SampleProp",
                        PropertyType = PropertyType.String,
                        DefaultValue = "Hello World"
                    }
                }
            };
            config.Configuration.AppSettings[config.ProjectName] = new List<SettingsConfig>(new[] { settingsConfig });
            var projectDir = new DirectoryInfo(config.IntermediateOutputPath).Parent.FullName;
            config.SolutionDirectory = config.ProjectDirectory = projectDir;

            var generator = new SecretsClassGenerator(config);
            var mergedSecrets = generator.GetMergedSecrets(settingsConfig, out var hasErrors);

            Assert.False(hasErrors);
            Assert.Single(mergedSecrets);
        }

        [Fact]
        public void GetsDefaultValueFromValueConfig()
        {
            var config = GetConfiguration();
            var settingsConfig = new SettingsConfig
            {
                Properties = new List<ValueConfig>
                {
                    new ValueConfig
                    {
                        Name = "SampleProp",
                        PropertyType = PropertyType.String,
                        DefaultValue = "Hello World"
                    }
                }
            };
            config.Configuration.AppSettings[config.ProjectName] = new List<SettingsConfig>(new[] { settingsConfig });
            var projectDir = new DirectoryInfo(config.IntermediateOutputPath).Parent.FullName;
            config.SolutionDirectory = config.ProjectDirectory = projectDir;

            var generator = new SecretsClassGenerator(config);
            var mergedSecrets = generator.GetMergedSecrets(settingsConfig, out var hasErrors);

            Assert.False(hasErrors);
            Assert.True(mergedSecrets.ContainsKey("SampleProp"));
            Assert.Equal("Hello World", mergedSecrets["SampleProp"]);
        }

        [Fact]
        public void GetsValuesFromPrefixedHostEnvironmentVariable()
        {
            var config = GetConfiguration();
            var settingsConfig = new SettingsConfig
            {
                Prefix = "SampleTest_",
                Properties = new List<ValueConfig>
                {
                    new ValueConfig
                    {
                        Name = "SampleProp1",
                        PropertyType = PropertyType.String
                    }
                }
            };
            config.Configuration.AppSettings[config.ProjectName] = new List<SettingsConfig>(new[] { settingsConfig });
            var projectDir = new DirectoryInfo(config.IntermediateOutputPath).Parent.FullName;
            config.SolutionDirectory = config.ProjectDirectory = projectDir;
            Environment.SetEnvironmentVariable("SampleTest_SampleProp1", nameof(GetsValuesFromPrefixedHostEnvironmentVariable), EnvironmentVariableTarget.Process);

            var generator = new SecretsClassGenerator(config);
            var mergedSecrets = generator.GetMergedSecrets(settingsConfig, out var hasErrors);
            Environment.SetEnvironmentVariable("SampleTest_SampleProp1", null, EnvironmentVariableTarget.Process);

            Assert.False(hasErrors);
            Assert.True(mergedSecrets.ContainsKey("SampleProp1"));
            Assert.Equal(nameof(GetsValuesFromPrefixedHostEnvironmentVariable), mergedSecrets["SampleProp1"]);
        }

        [Fact]
        public void GetsPrefixedValueOverExactMatch()
        {
            var config = GetConfiguration();
            var settingsConfig = new SettingsConfig
            {
                Prefix = "SampleTest_",
                Properties = new List<ValueConfig>
                {
                    new ValueConfig
                    {
                        Name = "SampleProp2",
                        PropertyType = PropertyType.String
                    }
                }
            };
            config.Configuration.AppSettings[config.ProjectName] = new List<SettingsConfig>(new[] { settingsConfig });
            var projectDir = new DirectoryInfo(config.IntermediateOutputPath).Parent.FullName;
            config.SolutionDirectory = config.ProjectDirectory = projectDir;
            Environment.SetEnvironmentVariable("SampleTest_SampleProp2", nameof(GetsValuesFromPrefixedHostEnvironmentVariable), EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("SampleProp2", "Wrong Value", EnvironmentVariableTarget.Process);

            var generator = new SecretsClassGenerator(config);
            var mergedSecrets = generator.GetMergedSecrets(settingsConfig, out var hasErrors);
            Environment.SetEnvironmentVariable("SampleTest_SampleProp2", null, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable("SampleProp2", null, EnvironmentVariableTarget.Process);

            Assert.False(hasErrors);
            Assert.True(mergedSecrets.ContainsKey("SampleProp2"));
            Assert.Equal(nameof(GetsValuesFromPrefixedHostEnvironmentVariable), mergedSecrets["SampleProp2"]);
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
