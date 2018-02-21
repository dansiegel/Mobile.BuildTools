using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Tests.Mocks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures
{
    public class SecretsClassGeneratorFixture
    {
        private ITestOutputHelper _testOutputHelper { get; }

        public SecretsClassGeneratorFixture(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private SecretsClassGenerator GetGenerator() =>
            new SecretsClassGenerator
            {
                
                Log = new XunitLog(_testOutputHelper)
            };

        [Fact]
        public void ProcessSecretHidesValueForSafeOutput()
        {
            var key = "TestKey";
            var value = "TestValue";
            var pair = new KeyValuePair<string, JToken>(key, value);
            var generator = GetGenerator();

            var output = generator.ProcessSecret(pair, true);
            Assert.DoesNotContain(value, output);
            Assert.Contains("*****", output);
        }

        [Fact]
        public void ProcessSecretGeneratesBooleanValue()
        {
            var key = "TestKey";
            var value = true;
            var pair = new KeyValuePair<string, JToken>(key, value);
            var generator = GetGenerator();

            var output = generator.ProcessSecret(pair);
            Assert.Contains($"const bool {key}", output);
        }

        [Fact]
        public void ProcessSecretGeneratesIntegerValue()
        {
            var key = "TestKey";
            var value = 2;
            var pair = new KeyValuePair<string, JToken>(key, value);
            var generator = GetGenerator();

            var output = generator.ProcessSecret(pair);
            Assert.Contains($"const int {key}", output);
        }

        [Fact]
        public void ProcessSecretGeneratesDoubleValue()
        {
            var key = "TestKey";
            var value = 2.2;
            var pair = new KeyValuePair<string, JToken>(key, value);
            var generator = GetGenerator();

            var output = generator.ProcessSecret(pair);
            Assert.Contains($"const double {key}", output);
        }

        [Fact]
        public void ProcessSecretGeneratesStringValue()
        {
            var key = "TestKey";
            var value = "TestValue";
            var pair = new KeyValuePair<string, JToken>(key, value);
            var generator = GetGenerator();

            var output = generator.ProcessSecret(pair);
            Assert.Contains($"const string {key}", output);
        }
    }
}
