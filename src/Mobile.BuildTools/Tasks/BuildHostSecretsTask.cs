using System.IO;
using System.Linq;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Generators.Secrets;

namespace Mobile.BuildTools.Tasks
{
    public class BuildHostSecretsTask : BuildToolsTaskBase
    {
        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            if (config.Configuration.ProjectSecrets.All(x => x.Value.Disable))
                return;

            var globalSecretsJson = Path.Combine(config.SolutionDirectory, "secrets.json");

            if (File.Exists(globalSecretsJson) || File.Exists(Path.Combine(ProjectDirectory, "secrets.json")))
            {
                Log.LogMessage("A secrets file already exists. Please delete the file to regenerate the secrets");
                return;
            }

            Log.LogMessage($"Output Path: {globalSecretsJson}");
            IGenerator generator = new BuildHostSecretsGenerator(this)
            {
                SecretsJsonFilePath = globalSecretsJson,
            };

            generator.Execute();
        }
    }
}
