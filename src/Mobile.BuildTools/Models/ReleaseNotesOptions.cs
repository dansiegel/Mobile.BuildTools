using System.ComponentModel;
using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class ReleaseNotesOptions : ToolItem
    {
        [DefaultValue(10)]
        [JsonProperty("maxDays")]
        public int MaxDays { get; set; }

        [DefaultValue(10)]
        [JsonProperty("maxCommit")]
        public int MaxCommit { get; set; }

        [DefaultValue(250)]
        [JsonProperty("characterLimit")]
        public int CharacterLimit { get; set; }

        [DefaultValue("releasenotes.txt")]
        [JsonProperty("filename")]
        public string FileName { get; set; }

        [DefaultValue(true)]
        [JsonProperty("createInRoot")]
        public bool CreateInRoot { get; set; }
    }
}
