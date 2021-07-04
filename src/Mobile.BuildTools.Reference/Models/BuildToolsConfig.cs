using System;
using System.Collections.Generic;
using System.ComponentModel;
using Mobile.BuildTools.Models.Secrets;
using Mobile.BuildTools.Models.Settings;
using Mobile.BuildTools.Reference.Models.Settings;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class BuildToolsConfig
    {
        private string _schema = "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json";
        [JsonProperty("$schema")]
        public string Schema
        {
            get => _schema;
            set { }
        }

        [Description("Configures the settings for bundling and compiling the app.config for use with the Mobile.BuildTools.Configuration package.")]
        [JsonProperty("appConfig", Required = Required.AllowNull)]
        public AppConfig AppConfig { get; set; }

        [Description("Confgures the Mobile.BuildTools to copy the generated APK/AAB or IPA & dSYM to the root directory making it easier to locate and stage the build artifacts.")]
        [JsonProperty("artifactCopy", Required = Required.AllowNull)]
        public ArtifactCopy ArtifactCopy { get; set; }

        [Description("Configures the Mobile.BuildTools to automatically version the build for Android and iOS targets.")]
        [JsonProperty("automaticVersioning", Required = Required.AllowNull)]
        public AutomaticVersioning AutomaticVersioning { get; set; }

        [Description("Configures the Mobile.BuildTools to compile SCSS files into Xamarin.Forms compliant CSS for styling your Xamarin.Forms application with CSS.")]
        [JsonProperty("css", Required = Required.AllowNull)]
        public XamarinCss Css { get; set; }

        [Description("Configures the Mobile.BuildTools to intelligently process image sources to be bundled into your Android and iOS application.")]
        [JsonProperty("images", Required = Required.AllowNull)]
        public ImageResize Images { get; set; }

        [Description("Configures the Mobile.BuildTools to process Tokens within the AndroidManifest.xml and Info.plist, replacing values like $AppName$ with a variable named AppName.")]
        [JsonProperty("manifests", Required = Required.AllowNull)]
        public TemplatedManifest Manifests { get; set; }

        [Description("Configures the Mobile.BuildTools to generate Release Notes for your build, based on the Git commit messages.")]
        [JsonProperty("releaseNotes", Required = Required.AllowNull)]
        public ReleaseNotesOptions ReleaseNotes { get; set; }

        [Obsolete]
        [Description("Note: This is obsolete, please use `appSettings`.")]
        [JsonProperty("projectSecrets", Required = Required.AllowNull)]
        public Dictionary<string, SecretsConfig> ProjectSecrets { get; set; }

        [Description("Replaces the former 'Secrets' API, with a newly generated AppSettings class. This will allow you to generate one or more configuration classes.")]
        [JsonProperty("appSettings", Required = Required.AllowNull)]
        public Dictionary<string, IEnumerable<SettingsConfig>> AppSettings { get; set; }

        [Description("Configures the Mobile.BuildTools with default non-sensitive environment values. If the value does not exist in the System Environment, this value will be used.")]
        [JsonProperty("environment", Required = Required.AllowNull)]
        public EnvironmentSettings Environment { get; set; }

        [Description("Configures the Mobile.BuildTools to automatically generate and include the google-services.json or GoogleService-Info.plist from an Environment variable. This can be either a raw string value or file location if using Secure Files.")]
        [JsonProperty("google", Required = Required.AllowNull)]
        public GoogleConfig Google { get; set; }

        [Description("Having issues with the Mobile.BuildTools. Enable the Debug property to help you get some additional debug output in the build logs to help identify configuration issues.")]
        [JsonProperty("debug", Required = Required.AllowNull)]
        public bool Debug { get; set; }
    }
}
