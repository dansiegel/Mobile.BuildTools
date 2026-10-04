using System;
using System.Text.RegularExpressions;

namespace Mobile.BuildTools.Utils;

public static class StringExtensions
{
    public static Platform GetTargetPlatform(this string framework)
    {
        if (string.IsNullOrWhiteSpace(framework))
            return Platform.Unsupported;
        var target = framework.ToLowerInvariant();
        if (target.StartsWith("net", StringComparison.Ordinal) && target.Contains("-"))
            target = target.Substring(target.IndexOf('-') + 1);
        target = Regex.Replace(target, @"\d.*$", string.Empty);
        return target switch
        {
            "monoandroid" or "xamarin.android" or "xamarinandroid" or "android" => Platform.Android,
            "xamarinios" or "xamarin.ios" or "ios" => Platform.iOS,
            "xamarintvos" or "xamarin.tvos" or "tvos" => Platform.TVOS,
            "uap" => Platform.UWP,
            "windows" or "win" => Platform.Windows,
            "xamarinmac" or "xamarin.mac" or "maccatalyst" or "macos" => Platform.macOS,
            "tizen" => Platform.Tizen,
            "browser" or "wasm" or "webassembly" => Platform.WebAssembly,
            _ => Platform.Unsupported
        };
    }
}
