using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Mobile.BuildTools.Build;

namespace Mobile.BuildTools.Generators.Manifests;

internal class DefaultTemplatedManifestGenerator : BaseTemplatedManifestGenerator
{
    public DefaultTemplatedManifestGenerator(IBuildConfiguration configuration) : base(configuration) { }
    public override string GetBundId() => string.Empty;
    protected override string SetAppBundleId(string manifest, string packageName) => manifest;

    protected override string TransformManifest(string manifest, IDictionary<string, string> variables)
    {
        using var document = JsonDocument.Parse(manifest);
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
            WriteValue(writer, document.RootElement, variables);
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private void WriteValue(Utf8JsonWriter writer, JsonElement value, IDictionary<string, string> variables)
    {
        switch (value.ValueKind)
        {
            case JsonValueKind.Object:
                writer.WriteStartObject();
                foreach (var property in value.EnumerateObject())
                {
                    writer.WritePropertyName(ReplaceTokens(property.Name, variables));
                    WriteValue(writer, property.Value, variables);
                }
                writer.WriteEndObject();
                break;
            case JsonValueKind.Array:
                writer.WriteStartArray();
                foreach (var item in value.EnumerateArray())
                    WriteValue(writer, item, variables);
                writer.WriteEndArray();
                break;
            case JsonValueKind.String:
                writer.WriteStringValue(ReplaceTokens(value.GetString(), variables));
                break;
            default:
                value.WriteTo(writer);
                break;
        }
    }
}
