using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using Mobile.BuildTools.Models;
using Mobile.BuildTools.Models.AppIcons;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Newtonsoft.Json.Serialization;
using System.Text.Json.Serialization;

namespace Mobile.BuildTools.SchemaGenerator
{
    public class Program
    {
        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

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

        public static JSchema CreateSchema(Type type)
        {
            var generator = new JSchemaGenerator
            {
                DefaultRequired = Newtonsoft.Json.Required.Default,
                ContractResolver = new ConfigurationContractResolver()
            };
            generator.GenerationProviders.Add(new PlatformArrayGenerationProvider());
            var schema = generator.Generate(type);
            if (type == typeof(BuildToolsConfig))
            {
                var images = schema.Properties["images"];
                images.Properties["optimizeLottie"].Type = JSchemaType.Boolean;
                images.Properties["optimizeLottie"].Default = new Newtonsoft.Json.Linq.JValue(true);
                images.Required.Remove("optimizeLottie");
            }
            return schema;
        }

        private void GenerateSchema(Type type, string fileName)
        {
            Console.WriteLine($"Generating schema for {type.Name} - {fileName}");
            File.WriteAllText(Path.Combine(OutputDirectory, fileName), CreateSchema(type).ToString());
        }

        private sealed class PlatformArrayGenerationProvider : StringEnumGenerationProvider
        {
            public override bool CanGenerateSchema(JSchemaTypeGenerationContext context) =>
                context.ObjectType == typeof(Mobile.BuildTools.Utils.Platform[]) || base.CanGenerateSchema(context);

            public override JSchema GetSchema(JSchemaTypeGenerationContext context)
            {
                if (context.ObjectType != typeof(Mobile.BuildTools.Utils.Platform[])) return base.GetSchema(context);
                // PlatformArrayJsonConverter accepts either a name or an array of names.
                var schema = new JSchema { Type = JSchemaType.String | JSchemaType.Array | JSchemaType.Null };
                schema.Items.Add(new JSchema { Type = JSchemaType.String });
                return schema;
            }
        }

        // The production models use System.Text.Json attributes, not Newtonsoft attributes.
        private sealed class ConfigurationContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization serialization)
            {
                var property = base.CreateProperty(member, serialization);
                property.PropertyName = member.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ??
                    System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(property.PropertyName);
                var ignore = member.GetCustomAttribute<System.Text.Json.Serialization.JsonIgnoreAttribute>();
                if (ignore?.Condition == JsonIgnoreCondition.Always) property.Ignored = true;
                return property;
            }
        }
    }
}
