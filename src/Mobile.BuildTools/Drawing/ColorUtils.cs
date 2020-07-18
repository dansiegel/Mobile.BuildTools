using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Mobile.BuildTools.Drawing
{
    internal static class ColorUtils
    {
        private static readonly Dictionary<string, Color> namedColors = typeof(Color).GetFields(BindingFlags.Public | BindingFlags.Static)
                   .Where(x => x.FieldType == typeof(Color))
                   .ToDictionary(x => x.Name, x => (Color)x.GetValue(null));

        public static bool TryParse(string input, out Color result)
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

            try
            {
                result = Rgba32.ParseHex(input);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
