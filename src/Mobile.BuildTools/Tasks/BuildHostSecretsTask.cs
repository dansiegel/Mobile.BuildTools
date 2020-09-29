using System.IO;
using System.Linq;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Generators.Secrets;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Tasks
{
    public class BuildHostSecretsTask : BuildToolsTaskBase
    {
        public string JsonSecretsFilePath { get; set; }

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            if (!string.IsNullOrEmpty(JsonSecretsFilePath) && File.Exists(JsonSecretsFilePath) && File.GetAttributes(JsonSecretsFilePath).HasFlag(FileAttributes.Normal))
                return;

            if (config.GetSecretsConfig().Disable || config.BuildingInsideVisualStudio)
                return;

            var secretsFile = Path.Combine(ProjectDirectory, "secrets.json");
            if (File.Exists(secretsFile))
            {
                Log.LogMessage("A secrets file already exists. Please delete the file to regenerate the secrets");
                return;
            }

            Log.LogMessage($"Output Path: {secretsFile}");
            IGenerator generator = new BuildHostSecretsGenerator(this)
            {
                SecretsJsonFilePath = secretsFile,
            };

            generator.Execute();
        }
    }
}
