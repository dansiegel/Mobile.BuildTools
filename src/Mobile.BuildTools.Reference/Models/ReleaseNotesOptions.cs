using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Mobile.BuildTools.Models
{
    public class ReleaseNotesOptions : ToolItem
    {
        [DefaultValue(10)]
        [Description("The number of days back to look when generating the Release Notes")]
        [JsonPropertyName("maxDays")]
        public int MaxDays { get; set; }

        [DefaultValue(10)]
        [Description("The maximum number of commits to lookup")]
        [JsonPropertyName("maxCommit")]
        public int MaxCommit { get; set; }

        [DefaultValue(250)]
        [Description("The maximum character limit for generated Release Notes")]
        [JsonPropertyName("characterLimit")]
        public int CharacterLimit { get; set; }

        [DefaultValue("releasenotes.txt")]
        [Description("The output filename such as 'ReleaseNotes.md' or 'ReleaseNotes.txt'")]
        [JsonPropertyName("filename")]
        public string FileName { get; set; }

        [DefaultValue(true)]
        [JsonPropertyName("createInRoot")]
        public bool CreateInRoot { get; set; }
    }
}
