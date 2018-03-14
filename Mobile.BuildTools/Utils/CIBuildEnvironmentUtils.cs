using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mobile.BuildTools.Utils
{
    public static class CIBuildEnvironmentUtils
    {
        public static bool IsAppCenter => Environment.GetEnvironmentVariables()
                                                     .Keys
                                                     .Cast<object>()
                                                     .Any(k => k.ToString() == "APPCENTER_BUILD_ID");
        public static bool IsAppVeyor => Environment.GetEnvironmentVariables()
                                                     .Keys
                                                     .Cast<object>()
                                                     .Any(k => k.ToString() == "APPVEYOR_BUILD_NUMBER");
        public static bool IsJenkins => Environment.GetEnvironmentVariables()
                                                     .Keys
                                                     .Cast<object>()
                                                     .Any(k => k.ToString() == "BUILD_NUMBER");
        public static bool IsVSTS => Environment.GetEnvironmentVariables()
                                                     .Keys
                                                     .Cast<object>()
                                                     .Any(k => k.ToString() == "BUILD_BUILDNUMBER");

        public static bool IsBuildHost => IsAppCenter || IsAppVeyor || IsJenkins || IsVSTS;

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
                else if(IsJenkins)
                {
                    return Environment.GetEnvironmentVariable("BUILD_NUMBER");
                }
                else if(IsVSTS)
                {
                    return Environment.GetEnvironmentVariable("BUILD_BUILDNUMBER");
                }

                return null;
            }
        }
    }
}
