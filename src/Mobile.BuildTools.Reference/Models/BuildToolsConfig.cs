using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Mobile.BuildTools.Models.Secrets;
using Mobile.BuildTools.Models.Settings;

namespace Mobile.BuildTools.Models
{
    [Description("Configures the Mobile.BuildTools. This file should be located in the solution root directory next to the solution file.")]
    public class BuildToolsConfig
    {
        private string _schema = "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json";
        [JsonPropertyName("$schema")]
        public string Schema
        {
            get => _schema;
            set { }
        }

        [Description("Configures the settings for bundling and compiling the app.config for use with the Mobile.BuildTools.Configuration package.")]
        [JsonPropertyName("appConfig")]
        public AppConfig AppConfig { get; set; }

        [Description("Configures the Mobile.BuildTools to copy the generated APK/AAB or IPA & dSYM to the root directory making it easier to locate and stage the build artifacts.")]
        [JsonPropertyName("artifactCopy")]
        public ArtifactCopy ArtifactCopy { get; set; }

        [Description("Configures the Mobile.BuildTools to automatically version the build for Android and iOS targets.")]
        [JsonPropertyName("automaticVersioning")]
        public AutomaticVersioning AutomaticVersioning { get; set; }

        [Description("Configures the Mobile.BuildTools to compile SCSS files into Xamarin.Forms compliant CSS for styling your Xamarin.Forms application with CSS.")]
        [JsonPropertyName("css")]
        public XamarinCss Css { get; set; }

        [Description("Configures the Mobile.BuildTools to intelligently process image sources to be bundled into your Android and iOS application.")]
        [JsonPropertyName("images")]
        public ImageResize Images { get; set; }

        [Description("Configures the Mobile.BuildTools to process Tokens within the AndroidManifest.xml and Info.plist, replacing values like $AppName$ with a variable named AppName.")]
        [JsonPropertyName("manifests")]
        public TemplatedManifest Manifests { get; set; }

        [Description("Configures the Mobile.BuildTools to generate Release Notes for your build, based on the Git commit messages.")]
        [JsonPropertyName("releaseNotes")]
        public ReleaseNotesOptions ReleaseNotes { get; set; }

        [Obsolete]
        [Description("Note: This is obsolete, please use `appSettings`.")]
        [JsonPropertyName("projectSecrets")]
        public Dictionary<string, SecretsConfig> ProjectSecrets { get; set; }

        [Description("Replaces the former 'Secrets' API, with a newly generated AppSettings class. This will allow you to generate one or more configuration classes.")]
        [JsonPropertyName("appSettings")]
        public Dictionary<string, IEnumerable<SettingsConfig>> AppSettings { get; set; }

        [Description("Configures the Mobile.BuildTools with default non-sensitive environment values. If the value does not exist in the System Environment, this value will be used.")]
        [JsonPropertyName("environment")]
        public EnvironmentSettings Environment { get; set; }

        [Description("Configures the Mobile.BuildTools to automatically generate and include the google-services.json or GoogleService-Info.plist from an Environment variable. This can be either a raw string value or file location if using Secure Files.")]
        [JsonPropertyName("google")]
        public GoogleConfig Google { get; set; }

        [Description("Having issues with the Mobile.BuildTools. Enable the Debug property to help you get some additional debug output in the build logs to help identify configuration issues.")]
        [JsonPropertyName("debug")]
        public bool Debug { get; set; }
    }
}
