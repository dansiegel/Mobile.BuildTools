namespace Mobile.BuildTools.Utils
{
    public static class StringExtensions
    {
        public static Platform GetTargetPlatform(this string framework)
        {
            switch (framework)
            {
                case "MonoAndroid":
                case "Xamarin.Android":
                case "monoandroid":
                case "xamarinandroid":
                case "xamarin.android":
                    return Platform.Android;
                case "Xamarin.iOS":
                case "xamarinios":
                case "xamarin.ios":
                    return Platform.iOS;
                case "win":
                case "uap":
                    return Platform.UWP;
                case "Xamarin.Mac":
                case "xamarinmac":
                case "xamarin.mac":
                    return Platform.macOS;
                case "Tizen":
                case "tizen":
                    return Platform.Tizen;
                default:
                    return Platform.Unsupported;

            }
        }
    }
}
