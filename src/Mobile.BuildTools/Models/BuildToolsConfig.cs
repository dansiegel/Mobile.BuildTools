using System.Collections.Generic;
using Mobile.BuildTools.Models.Secrets;

namespace Mobile.BuildTools.Models
{
    public class BuildToolsConfig
    {
        public ArtifactCopy ArtifactCopy { get; set; }

        public AutomaticVersioning AutomaticVersioning { get; set; }

        public XamarinCss Css { get; set; }

        public ImageResize Images { get; set; }

        public TemplatedManifest Manifests { get; set; }

        public ReleaseNotesOptions ReleaseNotes { get; set; }

        public IDictionary<string, SecretsConfig> ProjectSecrets { get; set; }

        public bool Debug { get; set; }
    }
}
