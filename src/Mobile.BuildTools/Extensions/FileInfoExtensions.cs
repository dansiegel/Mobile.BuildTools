using System.IO;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Extensions
{
    internal static class FileInfoExtensions
    {
        public static string ReadAllText(this FileInfo fi)
        {
            if (!fi.Exists)
                return null;

            using var stream = fi.OpenRead();
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public static T ReadJsonFile<T>(this FileInfo fi) =>
            JsonConvert.DeserializeObject<T>(fi.ReadAllText());
    }
}
