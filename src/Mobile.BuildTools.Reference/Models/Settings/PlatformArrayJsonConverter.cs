using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mobile.BuildTools.Utils;

namespace Mobile.BuildTools.Models.Secrets;

internal class PlatformArrayJsonConverter : JsonConverter<Platform[]>
{
    public override Platform[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Case 1: Null -> return empty array
        if (reader.TokenType == JsonTokenType.Null)
        {
            return [];
        }

        // Case 2: String -> return array of one element
        if (reader.TokenType == JsonTokenType.String)
        {
            var singleValue = reader.GetString();
            return [ConvertStringToPlatform(singleValue)];
        }

        // Case 3: Array of strings -> parse each one
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            var platforms = new System.Collections.Generic.List<Platform>();

            // Move into the array
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    break;
                }

                if (reader.TokenType == JsonTokenType.String)
                {
                    var stringValue = reader.GetString();
                    platforms.Add(ConvertStringToPlatform(stringValue));
                }
            }

            return [.. platforms.Distinct()];
        }

        // If we reach here, token type is unexpected, so throw an exception or 
        // decide on a fallback (e.g., returning empty or throwing).
        throw new JsonException($"Unexpected token type {reader.TokenType} when parsing Platform array.");
    }

    public override void Write(Utf8JsonWriter writer, Platform[] value, JsonSerializerOptions options)
    {
        if (value == null || value.Length == 0)
        {
            // If you want to write out an empty array as null, you could do:
            // writer.WriteNullValue();
            // But in most cases, you probably want an empty array:
            writer.WriteStartArray();
            writer.WriteEndArray();
            return;
        }

        // Otherwise, write each value as a string in an array
        writer.WriteStartArray();
        foreach (var platform in value)
        {
            writer.WriteStringValue(platform.ToString());
        }
        writer.WriteEndArray();
    }

    private Platform ConvertStringToPlatform(string stringValue)
    {
        if (stringValue == null)
        {
            return Platform.Unsupported;
        }

        return Enum.TryParse<Platform>(stringValue, ignoreCase: true, out var result)
            ? result
            : Platform.Unsupported;
    }
}
