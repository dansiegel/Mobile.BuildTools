using System;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Generators.Secrets;
using Mobile.BuildTools.Tests.Mocks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Mobile.BuildTools.Tests.Fixtures
{
    public class BuildHostSecretsGeneratorFixture : FixtureBase
    {
        private const string SecretsPrefix = "UNIT_TEST_";
        private const string SecretsJsonFilePath = "Samples/secrets.json";

        public BuildHostSecretsGeneratorFixture(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        private BuildHostSecretsGenerator GetGenerator() =>
            GetGenerator(GetConfiguration());

        private BuildHostSecretsGenerator GetGenerator(IBuildConfiguration config) =>
            new BuildHostSecretsGenerator(config)
            {
                SecretsJsonFilePath = SecretsJsonFilePath,
            };

        private void SetTestVariable() =>
            Environment.SetEnvironmentVariable($"{SecretsPrefix}Test1", "SomeValue");

        [Fact]
        public void DoesNotThrowException()
        {
            SetTestVariable();
            IGenerator generator = GetGenerator();
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
