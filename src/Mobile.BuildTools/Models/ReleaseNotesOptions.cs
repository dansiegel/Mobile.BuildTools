using Newtonsoft.Json;

namespace Mobile.BuildTools.Models
{
    public class ReleaseNotesOptions : ToolItem
    {
        [JsonProperty("maxDays")]
        public int MaxDays { get; set; }

        [JsonProperty("maxCommit")]
        public int MaxCommit { get; set; }

        [JsonProperty("characterLimit")]
        public int CharacterLimit { get; set; }

        [JsonProperty("filename")]
        public string FileName { get; set; }

        [JsonProperty("createInRoot")]
        public bool CreateInRoot { get; set; }
    }
}
