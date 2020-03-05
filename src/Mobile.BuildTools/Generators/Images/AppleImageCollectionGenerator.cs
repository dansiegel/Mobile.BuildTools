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

#if DEBUG
            if (outputFileName is null)
                System.Diagnostics.Debugger.Break();
#endif

           // Check for appiconset
           var assetsIconSetBasePath = Path.Combine(
               "Assets.xcassets",
               $"{outputFileName}.appiconset");
           var mediaRootIconSetBasePath = Path.Combine(
               "Media.xcassets",
               $"{outputFileName}.appiconset");
           var mediaIconSetBasePath = Path.Combine(
               "Resources",
               "Assets.xcassets",
               $"{outputFileName}.appiconset");
           if (Directory.Exists(Path.Combine(Build.ProjectDirectory, assetsIconSetBasePath)))
           {
               return GetAppIconSet(resource, assetsIconSetBasePath, outputFileName, masterScale);
           }
           else if (Directory.Exists(Path.Combine(Build.ProjectDirectory, mediaRootIconSetBasePath)))
           {
               return GetAppIconSet(resource, mediaRootIconSetBasePath, outputFileName, masterScale);
           }
           else if (Directory.Exists(Path.Combine(Build.ProjectDirectory, mediaIconSetBasePath)))
           {
               return GetAppIconSet(resource, mediaIconSetBasePath, outputFileName, masterScale);
           }
           else
           {
               Log.LogMessage($"Found image {resource.InputFilePath} -> Resources/{outputFileName}.png");
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

        protected override ResourceDefinition GetPlatformResourceDefinition(string filePath)
        {
            var fi = new FileInfo(filePath);
            if (fi.Name != "Contents.json")
                throw new Exception($"File not supported. {filePath}");

            var parentDirectory = fi.Directory.Name;
            if (Path.GetExtension(parentDirectory) != ".appiconset")
                throw new Exception($"The Contents.json must be located within an appiconset. '{filePath}");

            return new ResourceDefinition
            {
                InputFilePath = filePath,
                Ignore = false,
                Name = Path.GetFileNameWithoutExtension(parentDirectory),
                Scale = 1
            };
        }

        private IEnumerable<OutputImage> GetAppIconSet(ResourceDefinition resource, string basePath, string outputFileName, double masterScale)
        {
            var contentsJson = Path.Combine(Build.ProjectDirectory, basePath, "Contents.json");
            imageResourcePaths.Add(contentsJson);
            var iconset = JsonConvert.DeserializeObject<AppleIconSet>(
                File.ReadAllText(contentsJson));

            return iconset.Images
                .Where(x => !string.IsNullOrEmpty(x.FileName) &&
                            x.FileName.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase))
                .Select(x =>
            {
                var scale = int.Parse(x.Scale[0].ToString());
                var size = x.Size.Split('x');
                Log.LogMessage($"Found App Icon Set image {resource.InputFilePath} -> {basePath}/{x.FileName}");
                var outputFile = Path.Combine(Build.IntermediateOutputPath, basePath, x.FileName);
                var outputLink = Path.Combine(basePath, x.FileName);
                var watermarkFilePath = GetWatermarkFilePath(resource);
                var width = (int)(double.Parse(size[0]) * scale * masterScale);
                var height = (int)(double.Parse(size[1]) * scale * masterScale);
                return new OutputImage
                {
                    InputFile = resource.InputFilePath,
                    OutputFile = outputFile,
                    OutputLink = outputLink,
                    Width = width,
                    Height = height,
                    RequiresBackgroundColor = outputFileName == "AppIcon",
                    ShouldBeVisible = false,
                    WatermarkFilePath = watermarkFilePath
                };
            });
        }
    }
}
