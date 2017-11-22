using System;
using System.IO;
using Mobile.BuildTools.Generators;

namespace Mobile.BuildTools.Tools
{
    public class BuildHostSecretsTask : Microsoft.Build.Utilities.Task
    {
        public string SecretsPrefix { get; set; }

        public string SecretsJsonFilePath { get; set; }

        public string SdkShortFrameworkIdentifier { get; set; }

        public override bool Execute()
        {
            try
            {
                Log.LogMessage($"Output Path: {SecretsJsonFilePath}");
                if(string.IsNullOrWhiteSpace(SecretsJsonFilePath))
                {
                    Log.LogMessage("No Secrets file specified");
                }
                else if(File.Exists(SecretsJsonFilePath))
                {
                    Log.LogMessage("A secrets file already exists. Pleaes delete the file to regenerate the secrets");
                }
                else
                {
                    ValidateSecretsPrefix();
                    var generator = new BuildHostSecretsGenerator()
                    {
                        SecretsPrefix = SecretsPrefix,
                        SecretsJsonFilePath = SecretsJsonFilePath,
                        Log = Log
                    };

                    generator.Execute();
                }
            }
            catch(Exception e)
            {
                Log.LogErrorFromException(e);
                return false;
            }

            return true;
        }

        private void ValidateSecretsPrefix()
        {
            if (!string.IsNullOrWhiteSpace(SecretsPrefix)) return;

            switch (SdkShortFrameworkIdentifier)
            {
                case "monoandroid":
                case "xamarinandroid":
                case "xamarin.android":
                    SecretsPrefix = "DroidSecret_";
                    break;
                case "xamarinios":
                case "xamarin.ios":
                    SecretsPrefix = "iOSSecret_";
                    break;
                case "win":
                case "uap":
                    SecretsPrefix = "UWPSecret_";
                    break;
                case "xamarinmac":
                case "xamarin.mac":
                    SecretsPrefix = "MacSecret_";
                    break;
                case "tizen":
                    SecretsPrefix = "TizenSecret_";
                    break;
                default:
                    SecretsPrefix = "Secret_";
                    break;

            }
        }
    }
}