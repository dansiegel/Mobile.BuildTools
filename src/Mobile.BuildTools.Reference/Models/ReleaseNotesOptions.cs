using System.ComponentModel;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class ReleaseNotesOptions : ToolItem
    {
        [DefaultValue(10)]
        [JsonProperty("maxDays", Required = Required.AllowNull)]
        public int MaxDays { get; set; }

        [DefaultValue(10)]
        [JsonProperty("maxCommit", Required = Required.AllowNull)]
        public int MaxCommit { get; set; }

        [DefaultValue(250)]
        [JsonProperty("characterLimit", Required = Required.AllowNull)]
        public int CharacterLimit { get; set; }

        [DefaultValue("releasenotes.txt")]
        [JsonProperty("filename", Required = Required.AllowNull)]
        public string FileName { get; set; }

        [DefaultValue(true)]
        [JsonProperty("createInRoot", Required = Required.AllowNull)]
        public bool CreateInRoot { get; set; }
    }
}
