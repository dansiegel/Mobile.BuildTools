using System.IO;
using Android;
using Android.App;
using Android.Content;

[assembly: LinkerSafe]
namespace Mobile.BuildTools.Configuration
{
    public partial class ConfigurationManager
    {
        public static void Init(string config = "app.config") => Init(Application.Context, config);

        public static void Init(Context context, string config = "App.config")
        {
            using (var stream = new StreamReader(context.Assets.Open(config)))
                Init(stream);
        }
    }
}