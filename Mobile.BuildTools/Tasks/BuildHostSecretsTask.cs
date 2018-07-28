using System;
using System.IO;
using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Logging;

namespace Mobile.BuildTools.Tasks
{
    public class BuildHostSecretsTask : Microsoft.Build.Utilities.Task
    {
        public string SecretsPrefix { get; set; }

        public string SecretsJsonFilePath { get; set; }

        public string SdkShortFrameworkIdentifier { get; set; }

        public string DebugOutput { get; set; }

        public override bool Execute()
        {
            try
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
                    ValidateSecretsPrefix();
                    bool.TryParse(DebugOutput, out var debug);
                    var generator = new BuildHostSecretsGenerator()
                    {
                        SecretsPrefix = SecretsPrefix,
                        SecretsJsonFilePath = SecretsJsonFilePath,
                        DebugOutput = debug,
                        Log = (BuildHostLoggingHelper)Log
                    };

                    generator.Execute();
                }
            }
            catch (Exception e)
            {
                Log.LogErrorFromException(e);
                return false;
            }

            return true;
        }

        private void ValidateSecretsPrefix()
        {
            if (!string.IsNullOrWhiteSpace(SecretsPrefix)) return;

            SecretsPrefix = Utils.EnvironmentAnalyzer.GetSecretPrefix(SdkShortFrameworkIdentifier);
        }
    }
}