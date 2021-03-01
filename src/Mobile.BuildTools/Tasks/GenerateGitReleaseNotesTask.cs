using Microsoft.Build.Framework;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Tasks
{
    public class GenerateGitReleaseNotesTask : BuildToolsTaskBase
    {
        [Required]
        public string OutputPath { get; set; }

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            if (!EnvironmentAnalyzer.IsInGitRepo(config.ProjectDirectory))
            {
                Log.LogMessage($"The project {config.ProjectName} is not inside of an initialized git repository");
                return;
            }

            IGenerator generator = new ReleaseNotesGenerator(config, OutputPath);
            generator.Execute();
        }
    }
}
