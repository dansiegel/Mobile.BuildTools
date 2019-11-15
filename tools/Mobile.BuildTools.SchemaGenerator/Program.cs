using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace Mobile.BuildTools.SchemaGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var serializerSettings = Utils.ConfigHelper.GetSerializerSettings();
            var generator = new JSchemaGenerator()
            {
                ContractResolver = serializerSettings.ContractResolver
            };

            var schema = generator.Generate(typeof(Models.BuildToolsConfig));
            Console.WriteLine(schema);

            var basePath = args.FirstOrDefault() ?? ".";
            var schemaFile = Path.Combine(basePath, @"schema.json");
            // serialize JSchema to a string and then write string to a file
            File.WriteAllText(schemaFile, schema.ToString());
        }
    }
}
