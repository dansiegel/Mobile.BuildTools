namespace Mobile.BuildTools.Models
{
    public class AutomaticVersioning
    {
        public bool Disable { get; set; }
        public VersionBehavior Behavior { get; set; }
        public VersionEnvironment Environment { get; set; }
        public int VersionOffset { get; set; }
    }
}
