using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Models.AppIcons;
using Newtonsoft.Json.Schema.Generation;

namespace Mobile.BuildTools.SchemaGenerator
{
    public class Program
    {
        public static int Main(string[] args) =>
            CommandLineApplication.Execute<Program>(args);

        [Required]
        [Option(Description = "Output Directory")]
        public string OutputDirectory { get; set; }

        private int OnExecute()
        {
            if (!Directory.Exists(OutputDirectory))
            {
                Console.Error.WriteLine($"Output directory does not exist: {OutputDirectory}");
                return 1;
            }

            GenerateSchema(typeof(BuildToolsConfig), "buildtools.schema.json");
            GenerateSchema(typeof(ResourceDefinition), "resourceDefinition.schema.json");

            return 0;
        }

        private void GenerateSchema(Type type, string fileName)
        {
            Console.WriteLine($"Generating schema for {type.Name} - {fileName}");
            var schemaGenerator = new JSchemaGenerator
            {
                DefaultRequired = Newtonsoft.Json.Required.AllowNull,
            };
            schemaGenerator.GenerationProviders.Add(new StringEnumGenerationProvider());
            
            var schema = schemaGenerator.Generate(type);
            File.WriteAllText(Path.Combine(OutputDirectory, fileName), schema.ToString());
        }
    }
}
