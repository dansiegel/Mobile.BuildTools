using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Mobile.BuildTools.Generators.Images
{
    internal static class ImageFingerprint
    {
        public static string Create(object settings, IEnumerable<string> files)
        {
            // Include the actual task build, not just a package version that can be reused locally.
            var inputs = new List<string>
            {
                typeof(ImageFingerprint).Assembly.ManifestModule.ModuleVersionId.ToString(),
                JsonSerializer.Serialize(settings)
            };
            foreach (var file in files.Where(x => !string.IsNullOrWhiteSpace(x)).Select(Path.GetFullPath).Distinct().OrderBy(x => x, StringComparer.Ordinal))
            {
                inputs.Add(file);
                if (!File.Exists(file))
                    throw new FileNotFoundException("Image processing dependency was not found.", file);
                using var stream = File.OpenRead(file);
                using var sha = SHA256.Create();
                inputs.Add(BitConverter.ToString(sha.ComputeHash(stream)));
            }
            return Hash(JsonSerializer.Serialize(inputs));
        }

        public static string Hash(string value)
        {
            using var sha = SHA256.Create();
            return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(value))).Replace("-", "").ToLowerInvariant();
        }

        public static bool IsCurrent(string output, string fingerprint) =>
            File.Exists(output) && File.Exists(output + ".inputs") && File.ReadAllText(output + ".inputs") == fingerprint;
    }
}
