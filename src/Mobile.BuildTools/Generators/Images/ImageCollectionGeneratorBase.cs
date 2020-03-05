using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Models.AppIcons;
using Mobile.BuildTools.Tasks.Utils;
using Mobile.BuildTools.Utils;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Generators.Images
{
    internal abstract class ImageCollectionGeneratorBase : GeneratorBase<IReadOnlyList<OutputImage>>
    {
        private static object executeLock = new object();

        public IEnumerable<string> SearchFolders { get; set; }

        protected List<string> imageResourcePaths;
        private List<string> imageConfigurationPaths = new List<string>();
        public IReadOnlyList<string> ImageInputFiles
        {
            get
            {
                var imageInputs = new List<string>(imageResourcePaths);
                imageInputs.AddRange(imageConfigurationPaths);
                return imageInputs;
            }
        }

        public ImageCollectionGeneratorBase(IBuildConfiguration buildConfiguration)
            : base(buildConfiguration)
        {
        }

        protected override void ExecuteInternal()
        {
            imageResourcePaths = new List<string>();
            var outputImageFiles = new List<OutputImage>();
            var inputFileNames = new List<string>();
            foreach (var folder in SearchFolders)
            {
                foreach (var file in Directory.GetFiles(folder, "*.png", SearchOption.TopDirectoryOnly))
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    if (inputFileNames.Any(x => x == fileName))
                    {
                        var originalFile = imageResourcePaths.First(x => Path.GetFileNameWithoutExtension(x) == fileName);
                        Log.LogWarning($"Found duplicate input image '{fileName}'. Originial input path '{originalFile}', and duplicate file path '{file}'.");
                        continue;
                    }

                    imageResourcePaths.Add(file);
                    inputFileNames.Add(fileName);

                    var jsonConfig = Path.Combine(Path.GetDirectoryName(file), $"{fileName}.json");

                    if(!File.Exists(jsonConfig))
                    {
                        var assembly = GetType().Assembly;
                        using var stream = assembly.GetManifestResourceStream("Mobile.BuildTools.Resources.resourceDefinition.json");
                        using var reader = new StreamReader(stream);
                        File.WriteAllText(jsonConfig, reader.ReadToEnd());
                    }

                    inputFileNames.Add(jsonConfig);
                }
            }

            // HACK: Error thrown that source is modified while iterating.
            //var input = imageInputFiles.Select(x => x.Clone() as string);
            //foreach (var file in input)
            for(var i = 0; i < imageResourcePaths.Count; i++)
            {
                // We need to iterate a second time so we can be sure we are
                // tracking all of the image files
                // TODO: Add json config to File Inputs
                var resource = GetResourceDefinition(imageResourcePaths.ElementAt(i));
                if (resource.ShouldIgnore(Build.Platform))
                    continue;

                var output = GetOutputImages(resource);
                if(output != null && output.Any())
                    outputImageFiles.AddRange(output);
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

            throw new FileNotFoundException(configFileName);
        }

        private ResourceDefinition GetResourceDefinition(string filePath)
        {
            var fileName = GetImageConfigurationPath(filePath);
            var json = File.ReadAllText(fileName);
            var definition = JsonConvert.DeserializeObject<ResourceDefinition>(json, ConfigHelper.GetSerializerSettings());

            if (definition is null)
            {
                definition = new ResourceDefinition
                {
                    Name = Path.GetFileNameWithoutExtension(filePath)
                };
            }

            definition.InputFilePath = filePath;

            if (definition.Scale == 0)
            {
                definition.Scale = 1;
            }
            else if(definition.Scale > 1)
            {
                do
                {
                    definition.Scale /= 100;
                } while (definition.Scale > 1);
            }

            return definition;
        }

        protected abstract IEnumerable<OutputImage> GetOutputImages(ResourceDefinition resource);

        protected string GetWatermarkFilePath(ResourceDefinition resource)
        {
            if (string.IsNullOrEmpty(resource.WatermarkFile))
                return null;

            return ImageInputFiles.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == resource.WatermarkFile || Path.GetFileName(x) == resource.WatermarkFile);
        }
    }
}
