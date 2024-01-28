using System.Text.RegularExpressions;

namespace Mobile.BuildTools.Utils
{
    public static class StringExtensions
    {
        public static Platform GetTargetPlatform(this string framework)
        {
            // convert net6.0-ios or net7.0-ios to ios to ensure support across all .NET versions
            var targetPlatform = Regex.Replace(framework?.ToLower(), @"(net\d\d?\.\d\-)", string.Empty);
            return (targetPlatform?.ToLower()) switch
            {
                "monoandroid" or "xamarin.android" or "xamarinandroid" or "android" => Platform.Android,
                "xamarinios" or "xamarin.ios" or "ios" => Platform.iOS,
                "win" or "uap" or "windows" => Platform.UWP,
                "xamarinmac" or "xamarin.mac" or "maccatalyst" or "macos" => Platform.macOS,
                "tizen" => Platform.Tizen,
                _ => Platform.Unsupported,
            };
        }
    }
}
