using Xunit;

namespace Mobile.BuildTools.Configuration.Tests
{
    public class ConfigurationManagerTests
    {
        [Fact]
        public void ShouldGetValuesForAppSettings()
        {
            var manager = ConfigurationManager.Init();

            // setup
            const string expectedFoo = "my foo";
            const string expectedBar = "my bar";

            // assert
            Assert.Equal(expectedFoo, manager.AppSettings["foo"]);
            Assert.Equal(expectedBar, manager.AppSettings["bar"]);
            Assert.Equal(4, manager.AppSettings.Count);
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
            var manager = ConfigurationManager.Init(true);

            Assert.Equal("my foo", manager.AppSettings["foo"]);
            manager.Transform("foo");
            Assert.Equal("transformed", manager.AppSettings["foo"]);
            manager.Reset();
            Assert.Equal("my foo", manager.AppSettings["foo"]);
        }

        [Fact]
        public void SingleEnvironmentListed()
        {
            var manager = ConfigurationManager.Init(true);
            Assert.Single(manager.Environments);
        }

        [Fact]
        public void NoEnvironmentListedIfNotEnabled()
        {
            var manager =ConfigurationManager.Init();
            Assert.Empty(manager.Environments);
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("Foo")]
        [InlineData("app.foo.config")]
        [InlineData("app.Foo.config")]
        public void EnvironmentExists(string environment)
        {
            var manager = ConfigurationManager.Init(true);
            Assert.True(manager.EnvironmentExists(environment));
        }

        [Fact]
        public void EnvironmentDoesNotExist()
        {
            var manager = ConfigurationManager.Init(true);
            Assert.False(manager.EnvironmentExists("notValid"));
        }

        [Fact]
        public void EnvironmentDoesNotExistWhenEnvironmentsNotEnabled()
        {
            var manager = ConfigurationManager.Init();
            Assert.False(manager.EnvironmentExists("foo"));
        }

        [Fact]
        public void AppConfigTransformsForEnvironment()
        {
            var manager = ConfigurationManager.Init(true);
            Assert.Equal("my foo", manager.AppSettings["foo"]);
            Assert.DoesNotContain("Environment", manager.AppSettings.AllKeys);
            manager.Transform("foo");

            Assert.Equal(5, manager.AppSettings.Count);
            Assert.Contains("Environment", manager.AppSettings.AllKeys);
            Assert.Equal("Debug", manager.AppSettings["Environment"]);
            Assert.Equal("transformed", manager.AppSettings["foo"]);
        }
    }
}
