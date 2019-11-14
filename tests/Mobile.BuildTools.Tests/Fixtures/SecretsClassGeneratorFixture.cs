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
            var key = "TestKey";
            var value = "TestValue";
            var pair = new KeyValuePair<string, JToken>(key, value);
            var generator = GetGenerator();
            var config = new SecretsConfig()
            {
                { key, new ValueConfig { PropertyType = PropertyType.String } }
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
                { key, new ValueConfig { PropertyType = PropertyType.Bool } }
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
                { key, new ValueConfig { PropertyType = PropertyType.Int } }
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
                { key, new ValueConfig { PropertyType = PropertyType.Double } }
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
                { key, new ValueConfig { PropertyType = PropertyType.Int } }
            };

            var output = generator.ProcessSecret(pair, config, firstRun);
            Assert.Contains($"const string {key}", output);
        }

        [Fact]
        public void GeneratesCorrectNamespace_WithSpecifiedBaseNamespace()
        {
            var buildConfig = GetConfiguration();
            var generator = GetGenerator(buildConfig);
            generator.BaseNamespace = "Foo.Bar";
            generator.OutputPath = "file://Repos/AwesomeProject/Foo/Helpers";
            buildConfig.ProjectDirectory = "file://Repos/AwesomeProject/Foo";

            string @namespace = null;
            var exception = Record.Exception(() => @namespace = generator.GetNamespace());
            Assert.Null(exception);

            Assert.Equal("Foo.Bar.Helpers", @namespace);
        }

        [Theory]
        [MemberData(nameof(GetSecretsFromDataGenerator))]
        public void ProcessSecretValue(SecretValue secretValue)
        {
            var pair = new KeyValuePair<string, JToken>(TestKey, secretValue.Value);
            var generator = GetGenerator();
            var valueConfig = new ValueConfig
            {
                PropertyType = secretValue.Type,
                IsArray = secretValue.IsArray
            };
            var secretsConfig = new SecretsConfig { { TestKey, valueConfig } };
            var output = generator.ProcessSecret(pair, secretsConfig, secretValue.FirstRun);
            Assert.Contains($"const int {TestKey}", output);

            sbyte a = 3;
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
