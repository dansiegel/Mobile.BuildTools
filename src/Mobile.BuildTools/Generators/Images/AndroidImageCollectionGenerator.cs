using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Models.AppIcons;
using Mobile.BuildTools.Tasks.Utils;

namespace Mobile.BuildTools.Generators.Images
{
    internal class AndroidImageCollectionGenerator : ImageCollectionGeneratorBase
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

        protected override IEnumerable<OutputImage> GetOutputImages(ResourceDefinition resource)
        {
            (var outputFileName, var ignore, var scale) = resource.GetConfiguration(Platform.Android);
            if(ignore)
            {
                yield return default;
            }

#if DEBUG
            if (outputFileName is null)
                System.Diagnostics.Debugger.Break();
#endif

            var resourceType = $"{resource?.Android?.ResourceType ?? AndroidResource.Drawable}".ToLower();
            var platformSanitizedName = Regex.Replace(outputFileName, @"\s+", "-");

            if(!Path.HasExtension(platformSanitizedName))
            {
                platformSanitizedName += ".png";
            }

            foreach(var resolution in Resolutions)
            {
                Log.LogMessage($"Found image: {resource.InputFilePath} -> {resourceType}-{resolution.Key}/{platformSanitizedName}");
                yield return new OutputImage
                {
                    InputFile = resource.InputFilePath,
                    OutputFile = Path.Combine(Build.IntermediateOutputPath,
                                              "Resources",
                                              $"{resourceType}-{resolution.Key}",
                                              platformSanitizedName),
                    OutputLink = Path.Combine("Resources",
                                              $"{resourceType}-{resolution.Key}",
                                              platformSanitizedName),
                    Scale = scale * (resolution.Value / 4),
                    ShouldBeVisible = true,
                    WatermarkFilePath = GetWatermarkFilePath(resource)
                };
            }
        }
    }
}
