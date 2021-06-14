using System.Collections.Generic;
using Mobile.BuildTools.Models.Secrets;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class BuildToolsConfig
    {
#if !SCHEMAGENERATOR
        private string _schema = "https://mobilebuildtools.com/schemas/v2/buildtools.schema.json";
        [JsonProperty("$schema")]
        public string Schema
        {
            get => _schema;
            set { }
        }
#endif

        [JsonProperty("appConfig")]
        public AppConfig AppConfig { get; set; }

        [JsonProperty("artifactCopy")]
        public ArtifactCopy ArtifactCopy { get; set; }

        [JsonProperty("automaticVersioning")]
        public AutomaticVersioning AutomaticVersioning { get; set; }

        [JsonProperty("css")]
        public XamarinCss Css { get; set; }

        [JsonProperty("images")]
        public ImageResize Images { get; set; }

        [JsonProperty("manifests")]
        public TemplatedManifest Manifests { get; set; }

        [JsonProperty("releaseNotes")]
        public ReleaseNotesOptions ReleaseNotes { get; set; }

        [JsonProperty("projectSecrets")]
        public Dictionary<string, SecretsConfig> ProjectSecrets { get; set; }

        [JsonProperty("environment")]
        public EnvironmentSettings Environment { get; set; }

        [JsonProperty("google")]
        public GoogleConfig Google { get; set; }

        [JsonProperty("debug")]
        public bool Debug { get; set; }
    }
}
