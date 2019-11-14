using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators;

namespace Mobile.BuildTools.Tasks
{
    public class GenerateGitReleaseNotesTask : BuildToolsTaskBase
    {
        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            IGenerator generator = new ReleaseNotesGenerator(config);
            generator.Execute();
        }
    }
}
