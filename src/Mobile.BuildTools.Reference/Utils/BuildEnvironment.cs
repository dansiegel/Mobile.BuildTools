using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mobile.BuildTools.Models.Settings;

namespace Mobile.BuildTools.Utils
{
    public class BuildEnvironment
    {
        public bool Debug { get; set; }
        public string ProjectName { get; set; }
        public string RootNamespace { get; set; }
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
        public string BuildConfiguration { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Platform TargetPlatform { get; set; }
        public IEnumerable<SettingsConfig> GeneratedClasses { get; set; } = [];
        public IDictionary<string, string> Environment { get; set; } = new Dictionary<string, string>();
    }
}
