using System;
using System.IO;
using Mobile.BuildTools.Tasks.Utils;

namespace Mobile.BuildTools.Tests
{
    internal static class TestConstants
    {
        public static readonly string ImageDirectory = Path.Combine("Templates", "Images");
        public static readonly string AndroidImageDirectory = Path.Combine(ImageDirectory, "MonoAndroid");
        public static readonly string AppleImageDirectory = Path.Combine(ImageDirectory, "Xamarin.iOS");
        public static readonly string DebugImageDirectory = Path.Combine(ImageDirectory, "Debug");

        private const string androidIconFileName = "appicon.png";
        private const string appleIconFileName = "ios-icon.png";

        public static string GetPlatformImageDirectory(Platform platform) =>
            platform switch
            {
                Platform.Android => AndroidImageDirectory,
                Platform.iOS => AppleImageDirectory,
                Platform.macOS => AppleImageDirectory,
                Platform.TVOS => AppleImageDirectory,
                _ => throw new NotSupportedException(),
            };

        public static string GetPlatformIconPath(Platform platform) =>
            platform switch
            {
                Platform.Android => Path.Combine(AndroidImageDirectory, androidIconFileName),
                Platform.iOS => Path.Combine(AppleImageDirectory, appleIconFileName),
                Platform.macOS => Path.Combine(AppleImageDirectory, appleIconFileName),
                Platform.TVOS => Path.Combine(AppleImageDirectory, appleIconFileName),
                _ => throw new NotSupportedException()
            };
    }
}
