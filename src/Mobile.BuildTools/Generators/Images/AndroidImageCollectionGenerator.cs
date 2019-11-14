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
                return Array.Empty<OutputImage>();
            }

            var resourceType = $"{resource?.Android?.ResourceType ?? AndroidResource.Drawable}".ToLower();

            return Resolutions.Select(x =>
            {
                return new OutputImage
                {
                    InputFile = resource.InputFilePath,
                    OutputFile = Path.Combine("Resources",
                                              $"{resourceType}-{x.Key}",
                                              Regex.Replace(outputFileName, "\\w", "-")),
                    Scale = scale * (x.Value / 4),
                    ShouldBeVisible = true,
                    WatermarkFilePath = GetWatermarkFilePath(resource)
                };
            });
        }
    }
}
