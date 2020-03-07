using System;
using System.IO;
using Mobile.BuildTools.Tasks.Utils;

namespace Mobile.BuildTools.Tests
{
    internal static class TestConstants
    {
        public static readonly string ImageDirectory = Path.Join("Templates", "Images");
        public static readonly string AndroidImageDirectory = Path.Join(ImageDirectory, "MonoAndroid");
        public static readonly string AppleImageDirectory = Path.Join(ImageDirectory, "Xamarin.iOS");
        public static readonly string DebugImageDirectory = Path.Join(ImageDirectory, "Debug");

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
                Platform.Android => Path.Join(AndroidImageDirectory, androidIconFileName),
                Platform.iOS => Path.Join(AppleImageDirectory, appleIconFileName),
                Platform.macOS => Path.Join(AppleImageDirectory, appleIconFileName),
                Platform.TVOS => Path.Join(AppleImageDirectory, appleIconFileName),
                _ => throw new NotSupportedException()
            };
    }
}
