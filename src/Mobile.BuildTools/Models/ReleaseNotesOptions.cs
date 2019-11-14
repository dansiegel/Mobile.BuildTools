namespace Mobile.BuildTools.Models
{
    public class ReleaseNotesOptions
    {
        public bool Enabled { get; set; }
        public int MaxDays { get; set; }
        public int MaxCommit { get; set; }
        public int CharacterLimit { get; set; }
        public string FileName { get; set; }
        public bool CreateInRoot { get; set; }
    }
}
