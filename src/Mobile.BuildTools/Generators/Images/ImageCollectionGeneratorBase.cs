using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mobile.BuildTools.Build;
using Mobile.BuildTools.Logging;
using Mobile.BuildTools.Models.AppIcons;
using Mobile.BuildTools.Utils;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Generators.Images
{
    internal abstract class ImageCollectionGeneratorBase : GeneratorBase
    {
        public IEnumerable<string> SearchFolders { get; set; }

        protected List<string> imageInputFiles;
        public IReadOnlyList<string> ImageInputFiles => imageInputFiles;

        private List<OutputImage> outputImageFiles;

        public ImageCollectionGeneratorBase(IBuildConfiguration buildConfiguration)
            : base(buildConfiguration)
        {
        }

        public IReadOnlyList<OutputImage> OutputImageFiles => outputImageFiles;

        protected override void Execute()
        {
            imageInputFiles = new List<string>();
            outputImageFiles = new List<OutputImage>();
            var inputFileNames = new List<string>();
            foreach(var folder in SearchFolders)
            {
                foreach(var file in Directory.GetFiles(folder, "*.png", SearchOption.TopDirectoryOnly))
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    if (inputFileNames.Any(x => x == fileName))
                    {
                        var originalFile = imageInputFiles.First(x => Path.GetFileNameWithoutExtension(x) == fileName);
                        Log.LogWarning($"Found duplicate input image '{fileName}'. Originial input path '{originalFile}', and duplicate file path '{file}'.");
                        continue;
                    }

                    imageInputFiles.Add(file);
                    inputFileNames.Add(fileName);
                }
            }

            ImageInputFiles.Select(x =>
            {
                // We need to iterate a second time so we can be sure we are tracking all of the image files
                var resource = GetResourceDefinition(x);
                outputImageFiles.AddRange(GetOutputImages(resource));
                return x;
            });
        }

        private ResourceDefinition GetResourceDefinition(string filePath)
        {
            ResourceDefinition definition = null;
            var configJsonFilePath = Path.Combine((filePath), Path.GetFileNameWithoutExtension(filePath), ".json");
            if (File.Exists(configJsonFilePath))
            {
                Log.LogMessage($"Found JSON config for '{Path.GetFileName(filePath)}'");
                imageInputFiles.Add(configJsonFilePath);
                definition = JsonConvert.DeserializeObject<ResourceDefinition>(File.ReadAllText(configJsonFilePath), ConfigHelper.GetSerializerSettings());
            }
            else
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
