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

        protected List<string> imageInputFiles;
        public IReadOnlyList<string> ImageInputFiles => imageInputFiles;

        public ImageCollectionGeneratorBase(IBuildConfiguration buildConfiguration)
            : base(buildConfiguration)
        {
        }

        protected override void ExecuteInternal()
        {
            imageInputFiles = new List<string>();
            var outputImageFiles = new List<OutputImage>();
            var inputFileNames = new List<string>();
            foreach (var folder in SearchFolders)
            {
                foreach (var file in Directory.GetFiles(folder, "*.png", SearchOption.TopDirectoryOnly))
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
            for(var i = 0; i < imageInputFiles.Count; i++)
            {
                // We need to iterate a second time so we can be sure we are
                // tracking all of the image files
                var resource = GetResourceDefinition(imageInputFiles.ElementAt(i));
                if (resource.ShouldIgnore(Build.Platform))
                    continue;
                var output = GetOutputImages(resource);
                outputImageFiles.AddRange(output);
            }

            Outputs = outputImageFiles;
        }

        private ResourceDefinition GetResourceDefinition(string filePath)
        {
            ResourceDefinition definition = null;
            var fileName = Path.GetFileNameWithoutExtension(filePath) + ".json";

            foreach(var searchFilePath in SearchFolders.Select(x => Path.Combine(x, fileName)))
            {
                if(File.Exists(searchFilePath))
                {
                    Log.LogMessage($"Found JSON config for '{Path.GetFileName(filePath)}'");
                    imageInputFiles.Add(searchFilePath);
                    definition = JsonConvert.DeserializeObject<ResourceDefinition>(File.ReadAllText(searchFilePath), ConfigHelper.GetSerializerSettings());
                    break;
                }
            }
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
