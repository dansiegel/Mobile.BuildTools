using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SkiaSharp;

namespace Mobile.BuildTools.Drawing
{
    internal static class ColorUtils
    {
        private static readonly Dictionary<string, SKColor> namedColors = typeof(SKColors).GetFields(BindingFlags.Public | BindingFlags.Static)
                   .Where(x => x.FieldType == typeof(SKColor))
                   .ToDictionary(x => x.Name, x => (SKColor)x.GetValue(null), StringComparer.OrdinalIgnoreCase);

        public static bool TryParse(string input, out SKColor result)
        {
            result = default;

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            else if (namedColors.ContainsKey(input))
            {
                result = namedColors[input];
                return true;
            }

            return SKColor.TryParse(input, out result);
        }
    }
}
