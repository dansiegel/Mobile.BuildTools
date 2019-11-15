namespace Mobile.BuildTools.Models
{
    public class TemplatedManifest
    {
        public bool Disable { get; set; }
        public string Token { get; set; }
        public string VariablePrefix { get; set; }
        public bool MissingTokensAsErrors { get; set; }
    }
}
