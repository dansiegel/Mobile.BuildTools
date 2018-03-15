using Mobile.BuildTools.Generators;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Utils;
using Mobile.BuildTools.Versioning;
using System;

namespace Mobile.BuildTools.Tasks
{
    public class AutomaticBuildVersioningTask : Microsoft.Build.Utilities.Task
    {
        public string Behavior { get; set; }
        public string VersionEnvironment { get; set; }
        public string ProjectPath { get; set; }
        public string VersionOffset { get; set; }

        public string TargetFrameworkIdentifier { get; set; }
        public string SdkShortFrameworkIdentifier { get; set; }

        public string DebugOutput { get; set; }

        public override bool Execute()
        {
            try
            {
                var generator = GetGenerator();

                if(generator == null)
                {
                    Log.LogMessage($"The current target framework '{SdkShortFrameworkIdentifier}' is not supported for Automatic Versioning");
                    return true;
                }

                if(generator.Behavior == Versioning.Behavior.Off)
                {
                    Log.LogMessage("Automatic versioning has been disabled.");
                    return true;
                }

                if(CIBuildEnvironmentUtils.IsBuildHost && generator.VersionEnvironment == Versioning.VersionEnvironment.Local)
                {
                    Log.LogMessage("Your current settings are to only build on your local machine, however it appears you are building on a Build Host.");
                    return true;
                }

                Log.LogMessage("Executing Automatic Version Generator");
                generator.Execute();
            }
            catch (Exception e)
            {
                Log.LogMessage(e.ToString());
                //Log.LogErrorFromException(e);

                return false;
            }
            return true;
        }

        private BuildVersionGeneratorBase GetGenerator()
        {
            bool.TryParse(DebugOutput, out bool debug);
            int.TryParse(VersionOffset, out int versionOffset);
            Behavior behavior = Versioning.Behavior.Off;
            if(!string.IsNullOrWhiteSpace(Behavior))
            {
                Enum.TryParse(Behavior, out behavior);
            }
            VersionEnvironment versionEnvironment = Versioning.VersionEnvironment.All;
            if (!string.IsNullOrWhiteSpace(VersionEnvironment))
            {
                Enum.TryParse(VersionEnvironment, out versionEnvironment);
            }

            var framework = string.IsNullOrWhiteSpace(SdkShortFrameworkIdentifier) ? TargetFrameworkIdentifier : SdkShortFrameworkIdentifier;

            switch (framework)
            {
                case "MonoAndroid":
                case "Xamarin.Android":
                case "monoandroid":
                case "xamarinandroid":
                case "xamarin.android":
                    return new AndroidAutomaticBuildVersionGenerator
                    {
                        DebugOutput = debug,
                        Log = (BuildHostLoggingHelper)Log,
                        ProjectPath = ProjectPath,
                        VersionOffset = versionOffset,
                        Behavior = behavior,
                        VersionEnvironment = versionEnvironment
                    };
                case "Xamarin.iOS":
                case "xamarinios":
                case "xamarin.ios":
                    return new iOSAutomaticBuildVersionGenerator
                    {
                        DebugOutput = debug,
                        Log = (BuildHostLoggingHelper)Log,
                        ProjectPath = ProjectPath,
                        VersionOffset = versionOffset,
                        Behavior = behavior,
                        VersionEnvironment = versionEnvironment
                    };
                case "win":
                case "uap":
                    return null;
                case "Xamarin.Mac":
                case "xamarinmac":
                case "xamarin.mac":
                    return null;
                case "Tizen":
                case "tizen":
                    return null;
                default:
                    return null;

            }
        }
    }
}
