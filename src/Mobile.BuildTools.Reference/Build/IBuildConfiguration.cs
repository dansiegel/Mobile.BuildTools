using System.Collections.Generic;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Models.Settings;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Utils;
using Mobile.BuildTools.Reference.Models.Settings;

namespace Mobile.BuildTools.Build
{
    public interface IBuildConfiguration
    {
        string BuildConfiguration { get; }

        bool BuildingInsideVisualStudio { get; }

        IDictionary<string, string> GlobalProperties { get; }

        string ProjectName { get; }

        string ProjectDirectory { get; }

        string SolutionDirectory { get; }

        string IntermediateOutputPath { get; }

        ILog Logger { get; }

        BuildToolsConfig Configuration { get; }

        Platform Platform { get; }

        void SaveConfiguration();

        IEnumerable<SettingsConfig> GetSettingsConfig();
    }
}
