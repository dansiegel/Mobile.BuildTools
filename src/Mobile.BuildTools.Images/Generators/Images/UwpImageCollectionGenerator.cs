using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Models.AppIcons;

namespace Mobile.BuildTools.Generators.Images
{
    public class UwpImageCollectionGenerator : ImageCollectionGeneratorBase
    {
        private static readonly Dictionary<string, int> _squareLogos = new Dictionary<string, int>
        {
            { "LargeTile", 1240 },
            { "SmallTile", 284 },
            { "Square44x44Logo", 176 },
            { "Square150x150Logo", 600 },
            { "StoreLogo", 200 }
        };

        private readonly double[] _scales = new[] { 1.0, 0.5, 0.375, 0.3125, 0.25 };

        public UwpImageCollectionGenerator(IBuildConfiguration buildConfiguration)
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

            return config.ResourceType switch
            {
                PlatformResourceType.AllSquareTiles => GetAllSquareTileOutputImages(config),
                PlatformResourceType.SmallTile => GetSmallTileOutputImages(config),
                PlatformResourceType.SplashScreen => GetSplashScreenOutputImages(config),
                PlatformResourceType.SquareTile => GetSquareTileOutputImages(config),
                PlatformResourceType.WideTile => GetWideTileOutputImages(config),
                _ => GetDefaultOutputImages(config)
            };
        }

        private IEnumerable<OutputImage> GetAllSquareTileOutputImages(IImageResource config)
        {
            var outputImages = new List<OutputImage>(GetSmallTileOutputImages(config));
            outputImages.AddRange(GetSquareTileOutputImages(config));
            return outputImages;
        }

        private IEnumerable<OutputImage> GetSmallTileOutputImages(IImageResource config)
        {
            foreach (var scale in _scales)
            {
                var output = GetOutputImage(config, scale, "SmallTile");
                output.Height = (int)(284 * scale);
                output.Width = (int)(284 * scale);
                yield return output;
            }
        }

        private IEnumerable<OutputImage> GetSplashScreenOutputImages(IImageResource config)
        {
            foreach (var scale in _scales)
            {
                var output = GetOutputImage(config, scale, "SplashScreen");
                output.Height = (int)(1200 * scale);
                output.Width = (int)(2480 * scale);
                yield return output;
            }
        }

        private IEnumerable<OutputImage> GetSquareTileOutputImages(IImageResource config)
        {
            foreach ((var name, var size) in _squareLogos)
            {
                if (name == "SmallTile")
                    continue;

                foreach (var scale in _scales)
                {
                    var output = GetOutputImage(config, scale, name);
                    output.Height = (int)(size * scale);
                    output.Width = (int)(size * scale);
                    yield return output;
                }
            }

            foreach (var scale in _scales)
            {
                var size = (int)(256 * scale);
                var output = GetOutputImage(config, scale, $"Square44x44Logo.altform-unplated_targetsize-{size}");
                output.Height = size;
                output.Width = size;
                yield return output;
            }

            foreach (var scale in _scales)
            {
                var size = (int)(256 * scale);
                var output = GetOutputImage(config, scale, $"Square44x44Logo.targetsize-{size}");
                output.Height = size;
                output.Width = size;
                yield return output;
            }
        }

        private IEnumerable<OutputImage> GetWideTileOutputImages(IImageResource config)
        {
            foreach (var scale in _scales)
            {
                var output = GetOutputImage(config, scale, "Wide310x150Logo");
                output.Height = (int)(600 * scale);
                output.Width = (int)(1240 * scale);
                yield return output;
            }
        }

        private IEnumerable<OutputImage> GetDefaultOutputImages(IImageResource config)
        {
            foreach(var scale in _scales)
            {
                var output = GetOutputImage(config, scale, Path.GetFileNameWithoutExtension(config.SourceFile));
                output.Scale = scale;
                yield return output;
            }
        }

        private OutputImage GetOutputImage(IImageResource config, double scale, string outputName, bool formattedName = false)
        {
            var suffix = scale switch
            {
                1.0 => "scale-400",
                0.5 => "scale-200",
                0.375 => "scale-150",
                0.3125 => "scale-125",
                0.25 => "scale-100",
                _ => throw new NotImplementedException()
            };
            var fileName = formattedName ? $"{outputName}.png" : $"{outputName}.{suffix}.png";
            var outputFile = Path.Combine(Build.IntermediateOutputPath, "Assets", fileName);
            var outputLink = Path.Combine("Assets", fileName);
            return new OutputImage
            {
                InputFile = config.SourceFile,
                OutputFile = outputFile,
                OutputLink = outputLink,
                Watermark = config.Watermark,
                BackgroundColor = config.BackgroundColor,
                PaddingColor = config.PaddingColor,
                PaddingFactor = config.PaddingFactor,
                ShouldBeVisible = true,
                RequiresBackgroundColor = false,
                BuildAction = "Content"
            };
        }
    }
}
