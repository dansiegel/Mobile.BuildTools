#if !(SCHEMAGENERATOR || CLI_TOOL)
using System;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Extensions;

namespace Mobile.BuildTools.Models.AppIcons
{

    public partial class OutputImage
    {
        public ITaskItem ToTaskItem()
        {
            var item = new TaskItem(OutputFile);
            item.SetMetadata(nameof(InputFile), InputFile);
            item.SetMetadata(nameof(OutputLink), OutputLink);
            item.SetMetadata(nameof(Height), Height.ToString());
            item.SetMetadata(nameof(Width), Width.ToString());
            item.SetMetadata(nameof(RequiresBackgroundColor), RequiresBackgroundColor.ToString());
            item.SetMetadata(nameof(Scale), Scale.ToString());
            item.SetMetadata(nameof(ShouldBeVisible), ShouldBeVisible.ToString());
            item.SetMetadata(nameof(BackgroundColor), BackgroundColor);
            item.SetMetadata(nameof(PaddingFactor), PaddingFactor?.ToString());
            item.SetMetadata(nameof(PaddingColor), PaddingColor);

            if (Watermark != null)
            {
                item.SetMetadata("WatermarkSourceFile", Watermark.SourceFile);
                item.SetMetadata("WatermarkColors", Watermark.Colors is null || Watermark.Colors?.Count() == 0 ? null : string.Join(",", Watermark.Colors));
                item.SetMetadata("WatermarkPosition", Watermark.Position?.ToString());
                item.SetMetadata("WatermarkText", Watermark.Text);
                item.SetMetadata("WatermarkTextColor", Watermark.TextColor);
                item.SetMetadata("WatermarkFontFamily", Watermark.FontFamily);
                item.SetMetadata("WatermarkFontFile", Watermark.FontFile);
                item.SetMetadata("WatermarkOpacity", Watermark.Opacity?.ToString());
            }

            return item;
        }

        public static OutputImage FromTaskItem(ITaskItem item)
        {
            var paddingString = item.GetMetadata(nameof(PaddingFactor));
            var image = new OutputImage
            {
                InputFile = item.GetMetadata(nameof(InputFile)),
                OutputFile = item.ItemSpec,
                OutputLink = item.GetMetadata(nameof(OutputLink)),
                BackgroundColor = item.GetMetadata(nameof(BackgroundColor)),
                PaddingColor = item.GetMetadata(nameof(PaddingColor)),
                PaddingFactor = !string.IsNullOrEmpty(paddingString) && double.TryParse(paddingString, out var p) ? p : default
            };

            int.TryParse(item.GetMetadata(nameof(Height)), out var height);
            int.TryParse(item.GetMetadata(nameof(Width)), out var width);
            bool.TryParse(item.GetMetadata(nameof(RequiresBackgroundColor)), out var requiresBackgroundColor);
            bool.TryParse(item.GetMetadata(nameof(ShouldBeVisible)), out var shouldBeVisible);
            if(double.TryParse(item.GetMetadata(nameof(Scale)), out var scale) && (height == 0 || width == 0) && scale == 0)
            {
                scale = 1;
            }

            image.Height = height;
            image.Width = width;
            image.RequiresBackgroundColor = requiresBackgroundColor;
            image.ShouldBeVisible = shouldBeVisible;
            image.Scale = scale;

            var watermarkSourceFile = item.GetMetadata("WatermarkSourceFile");
            var watermarkText = item.GetMetadata("WatermarkText");
            var opacityString = item.GetMetadata("WatermarkOpacity");
            var opacity = double.TryParse(opacityString, out var op) ? op : Constants.DefaultOpacity;

            if (!string.IsNullOrEmpty(watermarkSourceFile))
            {
                image.Watermark = new WatermarkConfiguration
                {
                    SourceFile = watermarkSourceFile,
                    Opacity = opacity
                };
            }
            else if(!string.IsNullOrEmpty(watermarkText))
            {
                image.Watermark = new WatermarkConfiguration
                {
                    Text = watermarkText,
                    TextColor = item.GetMetadata("WatermarkTextColor", "White"),
                    FontFamily = item.GetMetadata("WatermarkFontFamily", "Arial"),
                    FontFile = item.GetMetadata("WatermarkFontFile"),
                    Position = item.GetEnumFromMetadata("WatermarkPosition", WatermarkPosition.BottomRight),
                    Opacity = opacity
                };
                var colors = item.GetMetadata("WatermarkColors", "Red,OrangeRed");
                image.Watermark.Colors = colors.Split(',').ToArray();
            }

            return image;
        }
    }
}
#endif
