namespace Mobile.BuildTools.Utils
{
    public static class StringExtensions
    {
        public static Platform GetTargetPlatform(this string framework)
        {
            switch (framework?.ToLower())
            {
                case "monoandroid":
                case "xamarin.android":
                case "xamarinandroid":
                case "net6.0-android":
                    return Platform.Android;
                case "xamarinios":
                case "xamarin.ios":
                case "net6.0-ios":
                    return Platform.iOS;
                case "win":
                case "uap":
                case "net6.0-windows":
                    return Platform.UWP;
                case "xamarinmac":
                case "xamarin.mac":
                case "net6.0-maccatalyst":
                case "net6.0-macos":
                    return Platform.macOS;
                case "tizen":
                case "net6.0-tizen":
                    return Platform.Tizen;
                default:
                    return Platform.Unsupported;

            }
        }
    }
}
