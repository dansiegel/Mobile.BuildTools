using System.IO;
using Foundation;

[assembly: LinkerSafe]
namespace Mobile.BuildTools.Configuration
{
    public partial class ConfigurationManager
    {
        public static void Init(string config = "Assets/App.config")
        {
            using (var stream = new StreamReader(config))
                Init(stream);
        }
    }
}