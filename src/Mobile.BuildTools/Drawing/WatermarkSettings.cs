using System;
using System.Collections.Generic;
using System.Linq;
using Mobile.BuildTools.Models.AppIcons;
using SkiaSharp;

namespace Mobile.BuildTools.Drawing
{
    public class WatermarkSettings
    {
        public IEnumerable<SKColor> Colors { get; set; }
        public WatermarkPosition Position { get; set; }

        public string Text { get; set; }
        public SKColor TextColor { get; set; }
        public SKTypeface Typeface { get; set; }

        public static WatermarkSettings FromConfig(WatermarkConfiguration config)
        {
            var settings = new WatermarkSettings
            {
                Position = config.Position ?? WatermarkPosition.BottomRight,
                Text = config.Text
            };

            if (config.Colors is null || config.Colors.Count() == 0)
            {
                settings.Colors = new[] { SKColors.Red, SKColors.Purple };
            }
            else
            {
                settings.Colors = config.Colors.Select(x => ColorUtils.TryParse(x, out var color) ? color : throw new Exception($"Cannot parse the color with value '{x}'."));
            }

            if (string.IsNullOrEmpty(config.TextColor))
            {
                settings.TextColor = SKColors.White;
            }
            else
            {
                settings.TextColor = ColorUtils.TryParse(config.TextColor, out var color) ? color : throw new Exception($"Cannot parse the text color with value '{config.TextColor}'.");
            }

            if (!string.IsNullOrEmpty(config.FontFile))
            {
                // TODO: Support remote http and local files
                settings.Typeface = SKTypeface.FromFile(config.FontFile);
            }
            else if (!string.IsNullOrEmpty(config.FontFamily))
            {
                settings.Typeface = SKTypeface.FromFamilyName(config.FontFamily);
            }
            else
            {
                settings.Typeface = SKTypeface.Default;
            }

            return settings;
        }
    }
}
