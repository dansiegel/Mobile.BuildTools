using System;
using Mobile.BuildTools.Generators.Secrets;
using Mobile.BuildTools.Tests.Mocks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures
{
    public class BuildHostSecretsGeneratorFixture
    {
        private const string SecretsPrefix = "UNIT_TEST_";
        private const string SecretsJsonFilePath = "Samples/secrets.json";

        private ITestOutputHelper _testOutputHelper { get; }

        public BuildHostSecretsGeneratorFixture(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private BuildHostSecretsGenerator GetGenerator() =>
            new BuildHostSecretsGenerator()
            {
                SecretsPrefix = SecretsPrefix,
                SecretsJsonFilePath = SecretsJsonFilePath,
                Log = new XunitLog(_testOutputHelper)
            };

        private void SetTestVariable() =>
            Environment.SetEnvironmentVariable($"{SecretsPrefix}Test1", "SomeValue");

        [Fact]
        public void DoesNotThrowException()
        {
            SetTestVariable();
            var generator = GetGenerator();
            var ex = Record.Exception(() => generator.Execute());

            if (ex != null)
            {
                _testOutputHelper.WriteLine(ex.ToString());
            }

            Assert.Null(ex);
        }

        [Fact]
        public void CreatesJObject()
        {
            SetTestVariable();
            var generator = GetGenerator();
            var secrets = generator.GetSecrets();

            var jsonObject = generator.GetJObjectFromSecrets(secrets);

            Assert.NotNull(jsonObject);
            Assert.IsType<JObject>(jsonObject);
            Assert.Equal("SomeValue", jsonObject["Test1"].ToString());

        }

        [Fact]
        public void RetrievesEnvironmentVariables()
        {
            SetTestVariable();
            var generator = GetGenerator();
            var secrets = generator.GetSecrets();

            Assert.NotNull(secrets);
            Assert.NotEmpty(secrets);
            foreach (var s in secrets)
            {
                _testOutputHelper.WriteLine($"{s.GetType()}: {s.Key}: {s.Value}");
                _testOutputHelper.WriteLine($"     ---- {Environment.GetEnvironmentVariable(s.ToString())}");
            }

            Assert.Contains($"Test1", secrets.Keys);
        }

        [Fact]
        public void JObjectIsSanitized()
        {
            var generator = GetGenerator();
            var secretValue = "TestSecret";
            var json = JObject.Parse($"{{\"Secret\":\"{secretValue}\"}}");
            Assert.Equal(secretValue, json["Secret"]);

            var sanitized = generator.SanitizeJObject(json);
            var secret = JsonConvert.DeserializeAnonymousType(sanitized, new { Secret = "" });
            Assert.NotEqual(secretValue, secret.Secret);
            Assert.Equal("*****", secret.Secret);
        }
    }
}
