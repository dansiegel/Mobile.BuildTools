using System;
using System.Collections.Generic;
using System.Text;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Tasks.Utils;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Tests.Mocks
{
    public class TestBuildConfiguration : IBuildConfiguration
    {
        public bool BuildingInsideVisualStudio => true;
        public IDictionary<string, string> GlobalProperties { get; } = new Dictionary<string, string>
        {
            { "ProjectName", "HelloWorld" }
        };

        public string ProjectDirectory { get; set; }
        public string SolutionDirectory { get; }
        public string IntermediateOutputPath { get; }
        public ILog Logger { get; set; }
        public BuildToolsConfig Configuration => ConfigHelper.GetConfig(".");
        public Platform Platform { get; set; }

        public void SaveConfiguration()
        {
        }
    }
}
