using System.Collections.Generic;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Models.Secrets;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Tests.Mocks
{
    public class TestBuildConfiguration : IBuildConfiguration
    {
        public TestBuildConfiguration()
        {
            ConfigHelper.SaveDefaultConfig(".");
            SecretsConfig = new SecretsConfig
            {
                ClassName = "Secrets",
                Delimiter = ";",
                Namespace = "Helpers",
                Prefix = "UNIT_TEST_",
                Properties = new List<ValueConfig>()
            };
        }

        public bool BuildingInsideVisualStudio => true;
        public IDictionary<string, string> GlobalProperties { get; } = new Dictionary<string, string>
        {
            { "ProjectName", "HelloWorld" }
        };

        public string ProjectDirectory { get; set; }
        public string ProjectName { get; set; } = "HelloWorld";
        public string SolutionDirectory { get; set; }
        public string IntermediateOutputPath { get; set; }
        public string BuildConfiguration { get; set; }
        public ILog Logger { get; set; }

        private BuildToolsConfig configuration;
        public BuildToolsConfig Configuration => configuration ?? (configuration = ConfigHelper.GetConfig("."));
        public Platform Platform { get; set; }

        public SecretsConfig GetSecretsConfig() => SecretsConfig;

        public void SaveConfiguration()
        {
        }

        public SecretsConfig SecretsConfig { get; }
    }
}
