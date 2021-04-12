//using System;
//using System.Collections.Generic;
//using System.Linq;
//using SixLabors.ImageSharp;
//using Mobile.BuildTools.Models.AppIcons;
//using SixLabors.Fonts;

//namespace Mobile.BuildTools.Drawing
//{
//    public class WatermarkSettings
//    {
//        public IEnumerable<Color> Colors { get; set; }
//        public WatermarkPosition Position { get; set; }

//        public string Text { get; set; }
//        public Color TextColor { get; set; }
//        public Font TextFont { get; set; }

//        public static WatermarkSettings FromConfig(WatermarkConfiguration config)
//        {
//            var settings = new WatermarkSettings
//            {
//                Position = config.Position ?? WatermarkPosition.BottomRight,
//                Text = config.Text
//            };

//            if(config.Colors is null || config.Colors.Count() == 0)
//            {
//                settings.Colors = new[] { Color.Red, Color.Purple };
//            }
//            else
//            {
//                settings.Colors = config.Colors.Select(x => ColorUtils.TryParse(x, out var color) ? color : throw new Exception($"Cannot parse the color with value '{x}'."));
//            }

//            if(string.IsNullOrEmpty(config.TextColor))
//            {
//                settings.TextColor = Color.White;
//            }
//            else
//            {
//                settings.TextColor = ColorUtils.TryParse(config.TextColor, out var color) ? color : throw new Exception($"Cannot parse the text color with value '{config.TextColor}'.");
//            }

//            if(!string.IsNullOrEmpty(config.FontFile))
//            {
//                // TODO: Support remote http and local files
//                var localFile = config.FontFile;
//                var fontCollection = new FontCollection();
//                FontFamily fontFamily = fontCollection.Install(localFile);
//                settings.TextFont = new Font(fontFamily, 10);
//            }
//            else if(!string.IsNullOrEmpty(config.FontFamily))
//            {
//                settings.TextFont = SystemFonts.CreateFont(config.FontFamily, 10);
//            }
//            else
//            {
//                settings.TextFont = SystemFonts.CreateFont("Arial", 10);
//            }

//            return settings;
//        }
//    }
//}
