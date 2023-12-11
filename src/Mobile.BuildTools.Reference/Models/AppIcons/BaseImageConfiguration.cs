using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Mobile.BuildTools.Models.AppIcons
{
    public class BaseImageConfiguration : IImageResource
    {
        [DefaultValue(false)]
        [Description("Will ignore this image asset. This may be used to ignore an image for a specific platform or in general if using the image as a watermark for other images.")]
        [JsonPropertyName("ignore")]
        public bool Ignore { get; set; }

        [Description("The name of the output image. This will default to the source image name.")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [DefaultValue(1.0)]
        [Description("The scale factor to scale the largest output image based on the size of the input image.")]
        [JsonPropertyName("scale")]
        public double Scale { get; set; }

        [Description("A color that should be used to apply as a background for a transparent source image. This is particularly useful for app icons on iOS. By default the AppIcon on iOS will get a White background if the source is transparent and no value is specified here. This can be a hex value or name of a standard color.")]
        [JsonPropertyName("backgroundColor")]
        public string BackgroundColor { get; set; }

        [Description("A specific override for the width of the largest output image.")]
        [JsonPropertyName("width")]
        public int? Width { get; set; }

        [Description("A specific override for the height of the largest output image.")]
        [JsonPropertyName("height")]
        public int? Height { get; set; }

        [Description("The padding factor will resize the canvas size by the specified factor. For example a factor of 1.5 will increase the overall size of the canvas. This will then be resized back to the original source image size and scaled appropriately. This is useful for Round Icons on Android.")]
        [JsonPropertyName("padFactor")]
        public double? PaddingFactor { get; set; }

        [Description("This will set the background color for the padded area to the specified color. This can be a hex value or name of standard colors.")]
        [JsonPropertyName("padColor")]
        public string PaddingColor { get; set; }

        [Description("Specifies a configuration for adding a watermark or banner on a specified image.")]
        [JsonPropertyName("watermark")]
        public WatermarkConfiguration Watermark { get; set; }

        [Description("Specifies the output resource type for the image. This typically should be used within a platform specific configuration.")]
        [JsonPropertyName("resourceType")]
        public PlatformResourceType ResourceType { get; set; }

        [JsonIgnore]
        public string SourceFile { get; set; }
    }
}
