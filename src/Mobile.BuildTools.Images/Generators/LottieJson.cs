using System;
using System.IO;
using System.Text.Json;

namespace Mobile.BuildTools.Generators.Images
{
    internal static class LottieJson
    {
        public static ReadOnlyMemory<byte> WithoutBom(byte[] input) =>
            input.Length >= 3 && input[0] == 0xef && input[1] == 0xbb && input[2] == 0xbf
                ? new ReadOnlyMemory<byte>(input, 3, input.Length - 3) : input;

        // Validate first, then remove only JSON whitespace outside strings. In particular,
        // never parse/write numbers: exponent spelling, negative zero and precision survive.
        public static byte[] Minify(byte[] input)
        {
            var json = WithoutBom(input);
            using (var document = JsonDocument.Parse(json)) { }
            using var output = new MemoryStream(input.Length);
            var quoted = false;
            var escaped = false;
            foreach (var value in json.Span)
            {
                if (quoted)
                {
                    output.WriteByte(value);
                    if (escaped) escaped = false;
                    else if (value == (byte)'\\') escaped = true;
                    else if (value == (byte)'"') quoted = false;
                }
                else if (value == (byte)'"')
                {
                    quoted = true;
                    output.WriteByte(value);
                }
                else if (value != 0x20 && value != 0x09 && value != 0x0a && value != 0x0d)
                    output.WriteByte(value);
            }
            return output.ToArray();
        }
    }
}
