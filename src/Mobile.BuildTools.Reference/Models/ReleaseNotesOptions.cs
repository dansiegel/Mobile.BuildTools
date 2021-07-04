using System.ComponentModel;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class ReleaseNotesOptions : ToolItem
    {
        [DefaultValue(10)]
        [Description("The number of days back to look when generating the Release Notes")]
        [JsonProperty("maxDays", NullValueHandling = NullValueHandling.Ignore)]
        public int MaxDays { get; set; }

        [DefaultValue(10)]
        [Description("The maximum number of commits to lookup")]
        [JsonProperty("maxCommit", NullValueHandling = NullValueHandling.Ignore)]
        public int MaxCommit { get; set; }

        [DefaultValue(250)]
        [Description("The maximum character limit for generated Release Notes")]
        [JsonProperty("characterLimit", NullValueHandling = NullValueHandling.Ignore)]
        public int CharacterLimit { get; set; }

        [DefaultValue("releasenotes.txt")]
        [Description("The output filename such as 'ReleaseNotes.md' or 'ReleaseNotes.txt'")]
        [JsonProperty("filename", NullValueHandling = NullValueHandling.Ignore)]
        public string FileName { get; set; }

        [DefaultValue(true)]
        [JsonProperty("createInRoot", NullValueHandling = NullValueHandling.Ignore)]
        public bool CreateInRoot { get; set; }
    }
}
