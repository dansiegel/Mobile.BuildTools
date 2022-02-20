using Microsoft.CodeAnalysis;
using ConfigAccessibility = Mobile.BuildTools.Models.Settings.Accessibility;

namespace Mobile.BuildTools.AppSettings.Extensions
{
    internal static class AccessibilityExtensions
    {
        public static Accessibility ToRoslynAccessibility(this ConfigAccessibility accessibility) =>
            accessibility switch
            {
                ConfigAccessibility.Public => Accessibility.Public,
                _ => Accessibility.Internal
            };
    }
}
