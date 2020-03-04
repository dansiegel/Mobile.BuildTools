using Xunit;

namespace Mobile.BuildTools.Configuration.Tests
{
    public class ConfigurationManagerTests
    {
        [Fact]
        public void ShouldGetValuesForAppSettings()
        {
            ConfigurationManager.Init();

            // setup
            const string expectedFoo = "my foo";
            const string expectedBar = "my bar";

            // assert
            Assert.Equal(expectedFoo, ConfigurationManager.AppSettings["foo"]);
            Assert.Equal(expectedBar, ConfigurationManager.AppSettings["bar"]);
            Assert.Equal(2, ConfigurationManager.AppSettings.Count);
        }

        [Fact]
        public void ShouldGetValuesForConnectionString()
        {
            ConfigurationManager.Init();

            // setup
            const string expectedProvider = "my provider";
            const string expectedString = "my connection string";

            // execute

            // assert
            Assert.Contains("test", ConfigurationManager.ConnectionStrings.Keys);
            var connectionString = ConfigurationManager.ConnectionStrings["test"];
            Assert.Equal(expectedProvider, connectionString.ProviderName);
            Assert.Equal(expectedString, connectionString.ConnectionString);
        }

        [Fact]
        public void CannotTransformIfNotEnabled()
        {
            ConfigurationManager.Init();
            Assert.Equal("my foo", ConfigurationManager.AppSettings["foo"]);

            var ex = Record.Exception(() => ConfigurationManager.Transform("foo"));
            Assert.Null(ex);

            Assert.Equal("my foo", ConfigurationManager.AppSettings["foo"]);
        }

        [Fact]
        public void TransformMaintainsCurrentInstance()
        {
            ConfigurationManager.Init(true);
            var current = ConfigurationManager.Current;

            Assert.Equal("my foo", current.AppSettings["foo"]);
            ConfigurationManager.Transform("foo");
            Assert.Equal("transformed", current.AppSettings["foo"]);
        }

        [Fact]
        public void ResetTransformsBackToDefault()
        {
            ConfigurationManager.Init(true);

            Assert.Equal("my foo", ConfigurationManager.AppSettings["foo"]);
            ConfigurationManager.Transform("foo");
            Assert.Equal("transformed", ConfigurationManager.AppSettings["foo"]);
            ConfigurationManager.Reset();
            Assert.Equal("my foo", ConfigurationManager.AppSettings["foo"]);
        }

        [Fact]
        public void SingleEnvironmentListed()
        {
            ConfigurationManager.Init(true);
            Assert.Single(ConfigurationManager.Environments);
        }

        [Fact]
        public void NoEnvironmentListedIfNotEnabled()
        {
            ConfigurationManager.Init();
            Assert.Empty(ConfigurationManager.Environments);
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("Foo")]
        [InlineData("app.foo.config")]
        [InlineData("app.Foo.config")]
        public void EnvironmentExists(string environment)
        {
            ConfigurationManager.Init(true);
            Assert.True(ConfigurationManager.EnvironmentExists(environment));
        }

        [Fact]
        public void EnvironmentDoesNotExist()
        {
            ConfigurationManager.Init(true);
            Assert.False(ConfigurationManager.EnvironmentExists("notValid"));
        }

        [Fact]
        public void EnvironmentDoesNotExistWhenEnvironmentsNotEnabled()
        {
            ConfigurationManager.Init();
            Assert.False(ConfigurationManager.EnvironmentExists("foo"));
        }

        [Fact]
        public void AppConfigTransformsForEnvironment()
        {
            ConfigurationManager.Init(true);
            Assert.Equal("my foo", ConfigurationManager.AppSettings["foo"]);
            Assert.DoesNotContain("Environment", ConfigurationManager.AppSettings.AllKeys);
            ConfigurationManager.Transform("foo");

            Assert.Equal(3, ConfigurationManager.AppSettings.Count);
            Assert.Contains("Environment", ConfigurationManager.AppSettings.AllKeys);
            Assert.Equal("Debug", ConfigurationManager.AppSettings["Environment"]);
            Assert.Equal("transformed", ConfigurationManager.AppSettings["foo"]);
        }
    }
}
