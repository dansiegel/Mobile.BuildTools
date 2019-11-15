using System.Collections.Generic;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators.Secrets;
using Mobile.BuildTools.Models.Secrets;
using Mobile.BuildTools.Tests.Mocks;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures
{
    public partial class SecretsClassGeneratorFixture : FixtureBase
    {
        public const string TestKey = nameof(TestKey);

        public SecretsClassGeneratorFixture(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        private SecretsClassGenerator GetGenerator() =>
            GetGenerator(GetConfiguration());

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
        [InlineData(false, true, PropertyType.Bool, "const bool[]", "False;True", "new[] { false, true }")]
        [InlineData(true, false, PropertyType.Bool, "const bool", "False", "false")]
        [InlineData(true, true, PropertyType.Bool, "const bool[]", "False;True", "new[] { false, true }")]
        // byte
        [InlineData(false, false, PropertyType.Byte, "const byte", "0", "0")]
        [InlineData(false, true, PropertyType.Byte, "const byte[]", "0;1", "new[] { 0, 1 }")]
        // char
        [InlineData(false, false, PropertyType.Char, "const char", "c", "c")]
        [InlineData(false, true, PropertyType.Char, "const char[]", "h;e;l;l;o", "new[] { 'h', 'e', 'l', 'l', 'o' }")]
        // DateTime
        [InlineData(false, false, PropertyType.DateTime, "static readonly System.DateTime", "July 11, 2019 08:00", "System.DateTime.Parse(\"July 11, 2019 08:00\")")]
        [InlineData(false, true, PropertyType.DateTime, "static readonly System.DateTime[]", "July 11, 2019 08:00;July 13, 2019 17:00", "new[] { System.DateTime.Parse(\"July 11, 2019 08:00\"), System.DateTime.Parse(\"July 13, 2019 17:00\") }")]
        // DateTimeOffset
        [InlineData(false, false, PropertyType.DateTimeOffset, "static readonly System.DateTimeOffset", "July 11, 2019 08:00", "System.DateTimeOffset.Parse(\"July 11, 2019 08:00\")")]
        [InlineData(false, true, PropertyType.DateTimeOffset, "static readonly System.DateTimeOffset[]", "July 11, 2019 08:00 -7;July 13, 2019 17:00 -7", "new[] { System.DateTimeOffset.Parse(\"July 11, 2019 08:00 -7\"), System.DateTimeOffset.Parse(\"July 13, 2019 17:00 -7\") }")]
        // Decimal
        [InlineData(false, false, PropertyType.Decimal, "const decimal", "5.3M", "5.3M")]
        [InlineData(false, true, PropertyType.Decimal, "const decimal[]", "5.3M;3.3M", "new[] { 5.3M, 3.3M }")]
        // Double
        [InlineData(false, false, PropertyType.Double, "const double", "5.3", "5.3")]
        [InlineData(true, false, PropertyType.Double, "const double", "5.3", "5.3")]
        [InlineData(false, true, PropertyType.Double, "const double[]", "5.3;3.3", "new[] { 5.3, 3.3 }")]
        [InlineData(true, true, PropertyType.Double, "const double[]", "5.3;3.3", "new[] { 5.3, 3.3 }")]
        // Float
        [InlineData(false, false, PropertyType.Float, "const float", "5.3F", "5.3F")]
        [InlineData(false, true, PropertyType.Float, "const float[]", "5.3F;3.3F", "new[] { 5.3F, 3.3F }")]
        // Guid
        [InlineData(false, false, PropertyType.Guid, "static readonly System.Guid", "0d1d64bf-66a7-4455-a94e-459aabf84cca", "new System.Guid(\"0d1d64bf-66a7-4455-a94e-459aabf84cca\")")]
        [InlineData(false, true, PropertyType.Guid, "static readonly System.Guid[]", "0d1d64bf-66a7-4455-a94e-459aabf84cca;f476d60d-1c71-4771-a0ce-d72ac7f3cd35", "new[] { new System.Guid(\"0d1d64bf-66a7-4455-a94e-459aabf84cca\"), new System.Guid(\"f476d60d-1c71-4771-a0ce-d72ac7f3cd35\") }")]
        // Int
        [InlineData(false, false, PropertyType.Int, "const int", "6", "6")]
        [InlineData(false, false, PropertyType.Int, "const int", "6.0", "6")]
        [InlineData(true, false, PropertyType.Int, "const int", "6", "6")]
        [InlineData(true, false, PropertyType.Int, "const int", "6.0", "6")]
        [InlineData(false, true, PropertyType.Int, "const int[]", "6;5", "new[] { 6, 5 }")]
        [InlineData(true, true, PropertyType.Int, "const int[]", "6;5", "new[] { 6, 5 }")]
        // Long
        [InlineData(false, false, PropertyType.Long, "const long", "5", "5")]
        [InlineData(false, true, PropertyType.Long, "const long[]", "5;6", "new[] { 5, 6 }")]
        // SByte
        [InlineData(false, false, PropertyType.SByte, "const sbyte", "3", "3")]
        [InlineData(false, true, PropertyType.SByte, "const sbyte[]", "2;3", "new[] { 2, 3 }")]
        // Short
        [InlineData(false, false, PropertyType.Short, "const short", "2", "2")]
        [InlineData(false, true, PropertyType.Short, "const short[]", "2;3", "new[] { 2, 3 }")]
        // String
        [InlineData(false, false, PropertyType.String, "const string", "Hello World", "Hello World")]
        [InlineData(false, true, PropertyType.String, "const string[]", "Hello;World", "new[] { \"Hello\", \"World\" }")]
        [InlineData(true, false, PropertyType.String, "const string", "Hello World", "Hello World")]
        [InlineData(true, true, PropertyType.String, "const string[]", "Hello;World", "new[] { \"Hello\", \"World\" }")]
        // TimeSpan
        [InlineData(false, false, PropertyType.TimeSpan, "static readonly System.TimeSpan", "6:12", "System.TimeSpan.Parse(\"6:12\")")]
        [InlineData(false, true, PropertyType.TimeSpan, "static readonly System.TimeSpan[]", "6:12;4:20", "new[] { System.TimeSpan.Parse(\"6:12\"), System.TimeSpan.Parse(\"4:20\") }")]
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
