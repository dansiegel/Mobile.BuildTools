using System.IO;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Generators.Secrets;

namespace Mobile.BuildTools.Tasks
{
    public class BuildHostSecretsTask : BuildToolsTaskBase
    {
        public string SecretsJsonFilePath { get; set; }

        internal override void ExecuteInternal(IBuildConfiguration config)
        {
            Log.LogMessage($"Output Path: {SecretsJsonFilePath}");
            if (string.IsNullOrWhiteSpace(SecretsJsonFilePath))
            {
                Log.LogMessage($"No Secrets file specified for '{SdkShortFrameworkIdentifier}'");
            }
            else if (File.Exists(SecretsJsonFilePath))
            {
                Log.LogMessage("A secrets file already exists. Pleaes delete the file to regenerate the secrets");
            }
            else
            {
                IGenerator generator = new BuildHostSecretsGenerator(this)
                {
                    SecretsJsonFilePath = SecretsJsonFilePath,
                };

                generator.Execute();
            }
        }
    }
}