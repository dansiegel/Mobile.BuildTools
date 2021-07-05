using System;
using System.Linq;

namespace Mobile.BuildTools.Utils
{
    public static class CIBuildEnvironmentUtils
    {
        public static readonly bool IsCI = IsCIInternal();

        public static readonly bool IsAppCenter = Environment.GetEnvironmentVariables()
                                                     .Keys
                                                     .Cast<object>()
                                                     .Any(k => k.ToString() == "APPCENTER_BUILD_ID");

        public static readonly bool IsAppVeyor = Environment.GetEnvironmentVariables()
                                                     .Keys
                                                     .Cast<object>()
                                                     .Any(k => k.ToString() == "APPVEYOR_BUILD_NUMBER");

        public static readonly bool IsTeamCity = Environment.GetEnvironmentVariables()
                                                     .Keys
                                                     .Cast<object>()
                                                     .Any(k => k.ToString() == "TEAMCITY_VERSION");

        public static readonly bool IsJenkins = Environment.GetEnvironmentVariables()
                                                     .Keys
                                                     .Cast<object>()
                                                     .Any(k => k.ToString() == "JENKINS_HOME");

        public static readonly bool IsAzureDevOps = Environment.GetEnvironmentVariables()
                                                     .Keys
                                                     .Cast<object>()
                                                     .Any(k => k.ToString() == "BUILD_BUILDNUMBER");

        public static readonly bool IsBitBucket = Environment.GetEnvironmentVariables()
                                                     .Keys
                                                     .Cast<object>()
                                                     .Any(k => k.ToString() == "BITBUCKET_BUILD_NUMBER");

        public static readonly bool IsGitHubActions = Environment.GetEnvironmentVariables()
                                                     .Keys
                                                     .Cast<object>()
                                                     .Any(k => k.ToString() == "GITHUB_WORKFLOW");

        public static bool IsBuildHost => 
            IsCI ||
            IsAppCenter ||
            IsAppVeyor ||
            IsJenkins ||
            IsAzureDevOps ||
            IsBitBucket ||
            IsGitHubActions;

        public static string BuildNumber
        {
            get
            {
                if(IsAppCenter)
                {
                    return Environment.GetEnvironmentVariable("APPCENTER_BUILD_ID");
                }
                else if(IsAppVeyor)
                {
                    return Environment.GetEnvironmentVariable("APPVEYOR_BUILD_NUMBER");
                }
                else if(IsBitBucket)
                {
                    return Environment.GetEnvironmentVariable("BITBUCKET_BUILD_NUMBER");
                }
                else if(IsJenkins || IsTeamCity)
                {
                    return Environment.GetEnvironmentVariable("BUILD_NUMBER");
                }
                else if (IsGitHubActions)
                {
                    return Environment.GetEnvironmentVariable("GITHUB_RUN_ID");
                }
                else if(IsAzureDevOps)
                {
                    return Environment.GetEnvironmentVariable("BUILD_BUILDNUMBER");
                }

                return null;
            }
        }

        private static bool IsCIInternal()
        {
            var isCIString = Environment.GetEnvironmentVariable("CI");
            if (string.IsNullOrEmpty(isCIString))
                return false;
            else if (bool.TryParse(isCIString, out var isCI))
                return isCI;

            return false;
        }
    }
}
