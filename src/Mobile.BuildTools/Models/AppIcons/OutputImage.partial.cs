using System;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mobile.BuildTools.Extensions;

namespace Mobile.BuildTools.Models.AppIcons
{
    public static class OutputImageExtensions
    {
        public static ITaskItem ToTaskItem(this OutputImage image)
        {
            var item = new TaskItem(image.OutputFile);
            item.SetMetadata(nameof(image.InputFile), image.InputFile);
            item.SetMetadata(nameof(image.OutputLink), image.OutputLink);
            item.SetMetadata(nameof(image.Height), image.Height.ToString());
            item.SetMetadata(nameof(image.Width), image.Width.ToString());
            item.SetMetadata(nameof(image.RequiresBackgroundColor), image.RequiresBackgroundColor.ToString());
            item.SetMetadata(nameof(image.Scale), image.Scale.ToString());
            item.SetMetadata(nameof(image.ShouldBeVisible), image.ShouldBeVisible.ToString());
            item.SetMetadata(nameof(image.BackgroundColor), image.BackgroundColor);
            item.SetMetadata(nameof(image.PaddingFactor), image.PaddingFactor?.ToString());
            item.SetMetadata(nameof(image.PaddingColor), image.PaddingColor);

            if (image.Watermark != null)
            {
                item.SetMetadata("WatermarkSourceFile", image.Watermark.SourceFile);
                item.SetMetadata("WatermarkColors", image.Watermark.Colors is null || image.Watermark.Colors?.Count() == 0 ? null : string.Join(",", image.Watermark.Colors));
                item.SetMetadata("WatermarkPosition", image.Watermark.Position?.ToString());
                item.SetMetadata("WatermarkText", image.Watermark.Text);
                item.SetMetadata("WatermarkTextColor", image.Watermark.TextColor);
                item.SetMetadata("WatermarkFontFamily", image.Watermark.FontFamily);
                item.SetMetadata("WatermarkFontFile", image.Watermark.FontFile);
                item.SetMetadata("WatermarkOpacity", image.Watermark.Opacity?.ToString());
            }

            return item;
        }

        public static OutputImage ToOutputImage(this ITaskItem item)
        {
            var paddingString = item.GetMetadata(nameof(OutputImage.PaddingFactor));
            var image = new OutputImage
            {
                InputFile = item.GetMetadata(nameof(OutputImage.InputFile)),
                OutputFile = item.ItemSpec,
                OutputLink = item.GetMetadata(nameof(OutputImage.OutputLink)),
                BackgroundColor = item.GetMetadata(nameof(OutputImage.BackgroundColor)),
                PaddingColor = item.GetMetadata(nameof(OutputImage.PaddingColor)),
                PaddingFactor = !string.IsNullOrEmpty(paddingString) && double.TryParse(paddingString, out var p) ? p : default
            };

            int.TryParse(item.GetMetadata(nameof(OutputImage.Height)), out var height);
            int.TryParse(item.GetMetadata(nameof(OutputImage.Width)), out var width);
            bool.TryParse(item.GetMetadata(nameof(OutputImage.RequiresBackgroundColor)), out var requiresBackgroundColor);
            bool.TryParse(item.GetMetadata(nameof(OutputImage.ShouldBeVisible)), out var shouldBeVisible);
            if(double.TryParse(item.GetMetadata(nameof(OutputImage.Scale)), out var scale) && (height == 0 || width == 0) && scale == 0)
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
