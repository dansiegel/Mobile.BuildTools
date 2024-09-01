using System.Collections.Generic;

namespace Mobile.BuildTools.Utils
{
    public class BuildEnvironment
    {
        public bool IsCI { get; set; }
        public bool IsAppCenter { get; set; }
        public bool IsAppVeyor { get; set; }
        public bool IsTeamCity { get; set; }
        public bool IsJenkins { get; set; }
        public bool IsAzureDevOps { get; set; }
        public bool IsBitBucket { get; set; }
        public bool IsGitHubActions { get; set; }
        public bool IsTravisCI { get; set; }
        public bool IsBuildHost { get; set; }
        public string BuildNumber { get; set; }
        public IDictionary<string, string> Environment { get; set; } = new Dictionary<string, string>();
    }
}
