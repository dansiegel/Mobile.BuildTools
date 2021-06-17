using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Models.AppIcons;

namespace Mobile.BuildTools.Generators.Images
{
    public class AndroidImageCollectionGenerator : ImageCollectionGeneratorBase
    {
        // https://developer.android.com/training/multiscreen/screendensities
        private static readonly IDictionary<string, double> Resolutions = new Dictionary<string, double>
        {
            { "ldpi", 0.75 },
            { "mdpi", 1 },
            { "hdpi", 1.5 },
            { "xhdpi", 2 },
            { "xxhdpi", 3 },
            { "xxxhdpi", 4 }
        };

        public AndroidImageCollectionGenerator(IBuildConfiguration buildConfiguration)
            : base(buildConfiguration)
        {
        }

        protected internal override IEnumerable<OutputImage> GetOutputImages(IImageResource config)
        {
            if(config.Ignore)
            {
                yield return default;
            }

#if DEBUG
            if (config.Name is null)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    System.Diagnostics.Debugger.Break();
                else if(!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    System.Diagnostics.Debugger.Launch();
            }
#endif

            var resourceType = config.ResourceType switch
            {
                PlatformResourceType.Mipmap => "mipmap",
                _ => "drawable"
            };
            var platformSanitizedName = Regex.Replace(config.Name, @"\s+", "_");

            if(!Path.HasExtension(platformSanitizedName))
            {
                platformSanitizedName += ".png";
            }
            else if(Path.GetExtension(platformSanitizedName).Equals(".jpg", StringComparison.InvariantCultureIgnoreCase))
            {
                platformSanitizedName = Regex.Replace(platformSanitizedName, ".jpg", ".png", RegexOptions.IgnoreCase);
            }

            foreach (var resolution in Resolutions)
            {
                Log.LogMessage($"Found image: {config.SourceFile} -> {resourceType}-{resolution.Key}/{platformSanitizedName}");
                yield return new OutputImage
                {
                    InputFile = config.SourceFile,
                    OutputFile = Path.Combine(Build.IntermediateOutputPath,
                                                "Resources",
                                                $"{resourceType}-{resolution.Key}",
                                                platformSanitizedName),
                    OutputLink = Path.Combine("Resources",
                                                $"{resourceType}-{resolution.Key}",
                                                platformSanitizedName),
                    Scale = config.Scale * (resolution.Value / 4),
                    ShouldBeVisible = true,
                    Watermark = config.Watermark,
                    BackgroundColor = config.BackgroundColor,
                    BuildAction = "AndroidResource"
                };
            }
        }
    }
}
