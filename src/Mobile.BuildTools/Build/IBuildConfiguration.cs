using System.Collections.Generic;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Models.Secrets;
using Mobile.BuildTools.Tasks.Utils;

namespace Mobile.BuildTools.Build
{
    internal interface IBuildConfiguration
    {
        //IBuildEngine4 BuildEngine4 { get; }

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

        SecretsConfig GetSecretsConfig();
    }
}
