using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Models.AppIcons;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Generators.Images
{
    public abstract class ImageCollectionGeneratorBase : GeneratorBase<IReadOnlyList<OutputImage>>
    {
        private static readonly string[] supportedExtensions = new[] { ".png", ".jpg", ".jpeg", ".svg" };

        public ImageCollectionGeneratorBase(IBuildConfiguration buildConfiguration)
            : base(buildConfiguration)
        {
        }

        public IEnumerable<string> SearchFolders { get; set; }

        protected List<string> imageResourcePaths;
        private List<string> imageConfigurationPaths = new List<string>();
        public IReadOnlyList<string> ImageInputFiles
        {
            get
            {
                var imageInputs = new List<string>(imageResourcePaths);
                imageInputs.AddRange(imageConfigurationPaths);
                return imageInputs.Distinct().ToArray();
            }
        }

        protected override void ExecuteInternal()
        {
            imageResourcePaths = new List<string>();
            imageConfigurationPaths = new List<string>();
            var outputImageFiles = new List<OutputImage>();
            var inputFileNames = new List<string>();
            foreach (var folder in SearchFolders)
            {
                var filesInFolder = Directory.GetFiles(folder, "*", SearchOption.TopDirectoryOnly)
                    .Where(x => supportedExtensions.Any(e => e.Equals(Path.GetExtension(x), StringComparison.InvariantCultureIgnoreCase)))
                    .OrderBy(x => x, StringComparer.Ordinal);
                foreach (var file in filesInFolder)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    if (inputFileNames.Any(x => x == fileName))
                    {
                        var originalFile = imageResourcePaths.First(x => Path.GetFileNameWithoutExtension(x) == fileName);
                        Log.LogWarning($"Found duplicate input image '{fileName}'. Original input path '{originalFile}', and duplicate file path '{file}'.");
                        continue;
                    }

                    imageResourcePaths.Add(file);
                    inputFileNames.Add(fileName);


                }
            }

            var inputList = imageResourcePaths.ToArray();
            foreach (var path in inputList)
            {
                // We need to iterate a second time so we can be sure we are
                // tracking all of the image files
                var resourceDefinition = GetResourceDefinition(path);
                var configs = resourceDefinition.GetConfigurations(Build.Platform);
                foreach (var config in configs)
                {
                    if (config.Ignore)
                        continue;

#if DEBUG
                    if (string.IsNullOrEmpty(config.Name))
                    {
                        if (System.Diagnostics.Debugger.IsAttached)
                            System.Diagnostics.Debugger.Break();
                        else if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                            System.Diagnostics.Debugger.Launch();
                    }
#endif

                    if (config.Watermark != null)
                    {
                        config.Watermark.SourceFile = GetWatermarkFilePath(config);
                        if (!string.IsNullOrEmpty(config.Watermark.FontFile))
                        {
                            config.Watermark.FontFile = ResolveDependency(config.Watermark.FontFile, config.SourceFile);
                            imageConfigurationPaths.Add(config.Watermark.FontFile);
                        }
                        if (!string.IsNullOrEmpty(config.Watermark.SourceFile))
                            imageConfigurationPaths.Add(config.Watermark.SourceFile);
                    }

                    var output = GetOutputImages(config);
                    if (output != null && output.Any())
                        outputImageFiles.AddRange(output);
                }
            }

            Outputs = outputImageFiles;
        }

        private string GetImageConfigurationPath(string filePath)
        {
            var configFileName = $"{Path.GetFileNameWithoutExtension(filePath)}.json";
            var locatedConfigs = new List<string>();
            foreach (var searchDir in SearchFolders)
            {
                var path = Path.Combine(searchDir, configFileName);
                if (File.Exists(path))
                    locatedConfigs.Add(path);
            }

            switch (locatedConfigs.Count)
            {
                case 1:
                    imageConfigurationPaths.AddRange(locatedConfigs);
                    return locatedConfigs.First();
                case 2:
                    imageConfigurationPaths.AddRange(locatedConfigs);
                    var filtered = locatedConfigs.Where(x => Path.GetDirectoryName(x) != Path.GetDirectoryName(filePath));
                    if (filtered.Count() == 1)
                        return filtered.First();
                    throw new Exception($"Unable to determine which configuration to use for the image '{filePath}'. {string.Join(", ", locatedConfigs)}");
            }

            if (locatedConfigs.Count > 2)
                throw new Exception($"Unable to determine which configuration to use. More than 2 configuration files were found for the image '{filePath}', {string.Join(", ", locatedConfigs)}");

            return null;
        }

        protected virtual ResourceDefinition GetPlatformResourceDefinition(string filePath) => throw new NotImplementedException();

        protected internal ResourceDefinition GetResourceDefinition(string filePath)
        {
            if (!IsSupportedExtension(filePath))
                return GetPlatformResourceDefinition(filePath);

            var fileName = GetImageConfigurationPath(filePath);
            var definition = fileName is null ? null :
                JsonSerializer.Deserialize<ResourceDefinition>(File.ReadAllText(fileName), ConfigHelper.GetSerializerSettings());

            if (definition is null)
            {
                definition = new ResourceDefinition
                {
                    Name = Path.GetFileNameWithoutExtension(filePath)
                };
            }

            definition.SourceFile = filePath;

            if (definition.Scale == 0)
            {
                definition.Scale = 1;
            }
            else if (definition.Scale > 1)
            {
                do
                {
                    definition.Scale /= 100;
                } while (definition.Scale > 1);
            }

            return definition;
        }

        protected internal abstract IEnumerable<OutputImage> GetOutputImages(IImageResource resource);

        protected string GetWatermarkFilePath(IImageResource resource)
        {
            var fileName = resource.Watermark?.SourceFile;
            if (string.IsNullOrEmpty(fileName))
                return null;

            var found = ImageInputFiles.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == fileName || Path.GetFileName(x) == fileName);
            return found ?? ResolveDependency(fileName, resource.SourceFile);
        }

        protected static bool IsSupportedExtension(string path) =>
            supportedExtensions.Any(e => e.Equals(Path.GetExtension(path), StringComparison.InvariantCultureIgnoreCase));

        private string ResolveDependency(string path, string source)
        {
            if (File.Exists(path))
                return Path.GetFullPath(path);
            foreach (var folder in SearchFolders.Concat(new[] { Path.GetDirectoryName(source), Build.ProjectDirectory }))
            {
                var candidate = Path.Combine(folder, path.Replace('\\', Path.DirectorySeparatorChar));
                if (File.Exists(candidate))
                    return candidate;
            }
            throw new FileNotFoundException("Image dependency was not found.", path);
        }
    }
}
