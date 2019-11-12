namespace Mobile.BuildTools.Models
{
    public class AutomaticVersioning
    {
        public VersioningBehavior Behavior { get; set; }
        public VersioningEnvironment Environment { get; set; }
        public int VersionOffset { get; set; }
    }
}
