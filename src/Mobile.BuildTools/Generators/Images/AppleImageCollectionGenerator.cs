using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Models.AppIcons;
using Mobile.BuildTools.Tasks.Utils;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Generators.Images
{
    internal class AppleImageCollectionGenerator : ImageCollectionGeneratorBase
    {
        private static readonly IDictionary<string, double> ResourceSizes = new Dictionary<string, double>
        {
            { "@1x", 1.0 / 3.0 },
            { "@2x", 2.0 / 3.0 },
            { "@3x", 1 }
        };

        public AppleImageCollectionGenerator(IBuildConfiguration buildConfiguration)
            : base(buildConfiguration)
        {
        }

        protected override IEnumerable<OutputImage> GetOutputImages(ResourceDefinition resource)
        {
            (var outputFileName, var ignore, var masterScale) = resource.GetConfiguration(Platform.iOS);
            if (ignore)
            {
                return Array.Empty<OutputImage>();
            }

            // Check for appiconset
            var basePath = Path.Combine(
                "Assets.xcassets",
                $"{outputFileName}.appiconset");
            if (Directory.Exists(Path.Combine(Build.ProjectDirectory, basePath)))
            {
                var contentsJson = Path.Combine(basePath, "Contents.json");
                imageInputFiles.Add(contentsJson);
                var iconset = JsonConvert.DeserializeObject<AppleIconSet>(
                    File.ReadAllText(contentsJson));

                return iconset.Images.Select(x =>
                {
                    var scale = int.Parse(x.Scale[0].ToString());
                    var size = x.Size.Split('x');
                    return new OutputImage
                    {
                        InputFile = resource.InputFilePath,
                        OutputFile = Path.Combine(Build.IntermediateOutputPath, basePath, x.FileName),
                        OutputLink = Path.Combine(basePath, x.FileName),
                        Width = (int)(int.Parse(size[0]) * scale * masterScale),
                        Height = (int)(int.Parse(size[1]) * scale * masterScale),
                        RequiresBackgroundColor = outputFileName == "AppIcon",
                        ShouldBeVisible = false,
                        WatermarkFilePath = GetWatermarkFilePath(resource)
                    };
                });
            }
            else
            {
                // Generate App Resources
                return ResourceSizes.Select(x => new OutputImage
                {
                    InputFile = resource.InputFilePath,
                    OutputFile = Path.Combine(Build.IntermediateOutputPath, "Resources", $"{outputFileName}{x.Key}.png"),
                    OutputLink = Path.Combine("Resources", $"{outputFileName}{x.Key}.png"),
                    Scale = masterScale * x.Value,
                    ShouldBeVisible = true,
                    WatermarkFilePath = GetWatermarkFilePath(resource)
                });
            }
        }
    }
}
