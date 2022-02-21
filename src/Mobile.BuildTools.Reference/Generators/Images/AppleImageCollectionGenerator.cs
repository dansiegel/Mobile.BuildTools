using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Models.AppIcons;
using System.Text.Json;

namespace Mobile.BuildTools.Generators.Images
{
    public class AppleImageCollectionGenerator : ImageCollectionGeneratorBase
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

        protected internal override IEnumerable<OutputImage> GetOutputImages(IImageResource config)
        {
           if (config.Ignore)
           {
               return Array.Empty<OutputImage>();
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

            // Check for appiconset
            var assetsIconSetBasePath = Path.Combine(
               "Assets.xcassets",
               $"{config.Name}.appiconset");
           var mediaRootIconSetBasePath = Path.Combine(
               "Media.xcassets",
               $"{config.Name}.appiconset");
           var mediaIconSetBasePath = Path.Combine(
               "Resources",
               "Assets.xcassets",
               $"{config.Name}.appiconset");
           if (Directory.Exists(Path.Combine(Build.ProjectDirectory, assetsIconSetBasePath)))
           {
               return GetAppIconSet(config, assetsIconSetBasePath);
           }
           else if (Directory.Exists(Path.Combine(Build.ProjectDirectory, mediaRootIconSetBasePath)))
           {
               return GetAppIconSet(config, mediaRootIconSetBasePath);
           }
           else if (Directory.Exists(Path.Combine(Build.ProjectDirectory, mediaIconSetBasePath)))
           {
               return GetAppIconSet(config, mediaIconSetBasePath);
           }
           else
           {
                Log.LogMessage($"Found image {config.SourceFile} -> Resources/{config.Name}.png");
                // Generate App Resources
                return ResourceSizes.Select(x => new OutputImage
                {
                    InputFile = config.SourceFile,
                    OutputFile = Path.Combine(Build.IntermediateOutputPath, "Resources", $"{config.Name}{x.Key}.png"),
                    OutputLink = Path.Combine("Resources", $"{config.Name}{x.Key}.png"),
                    Scale = config.Scale * x.Value,
                    ShouldBeVisible = true,
                    Watermark = config.Watermark,
                    BackgroundColor = config.BackgroundColor,
                    BuildAction = "BundleResource"
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
                SourceFile = filePath,
                Ignore = false,
                Name = Path.GetFileNameWithoutExtension(parentDirectory),
                Scale = 1
            };
        }

        private IEnumerable<OutputImage> GetAppIconSet(IImageResource resource, string basePath)
        {
            var contentsJson = Path.Combine(Build.ProjectDirectory, basePath, "Contents.json");
            if (!imageResourcePaths.Any(x => x == contentsJson))
                imageResourcePaths.Add(contentsJson);
            var iconset = JsonSerializer.Deserialize<AppleIconSet>(
                File.ReadAllText(contentsJson));

            var iconsetImages = new List<AppleIconSetImage>();
            foreach(var image in iconset.Images)
            {
                // Require png file name
                if (string.IsNullOrEmpty(image.FileName) || Path.GetExtension(image.FileName) != ".png")
                    continue;

                // Skip duplicate resources
                if (iconsetImages.Any(x => x.FileName == image.FileName))
                    continue;

                iconsetImages.Add(image);
                yield return GetOutputImage(image, resource, basePath, resource.Name);
            }
        }

        private OutputImage GetOutputImage(AppleIconSetImage x, IImageResource resource, string basePath, string outputFileName)
        {
            var scale = int.Parse(x.Scale[0].ToString());
            var size = x.Size.Split('x');
            Log.LogMessage($"Found App Icon Set image {resource.SourceFile} -> {basePath}{Path.DirectorySeparatorChar}{x.FileName}");
            var outputFile = Path.Combine(Build.IntermediateOutputPath, basePath, x.FileName);
            var outputLink = Path.Combine(basePath, x.FileName);
            var watermarkFilePath = GetWatermarkFilePath(resource);
            var width = (int)(double.Parse(size[0]) * scale);
            var height = (int)(double.Parse(size[1]) * scale);
            return new OutputImage
            {
                InputFile = resource.SourceFile,
                OutputFile = outputFile,
                OutputLink = outputLink,
                Width = width,
                Height = height,
                RequiresBackgroundColor = outputFileName == "AppIcon",
                ShouldBeVisible = false,
                //WatermarkFilePath = watermarkFilePath,
                Watermark = resource.Watermark,
                BackgroundColor = resource.BackgroundColor,
                BuildAction = "ImageAsset"
            };
        }
    }
}
