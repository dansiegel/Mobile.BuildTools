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
