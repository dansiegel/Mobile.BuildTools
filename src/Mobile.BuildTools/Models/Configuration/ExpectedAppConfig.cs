namespace Mobile.BuildTools.Models.Configuration
{
    internal class ExpectedAppConfig
    {
        public string OutputPath { get; set; }
        public string SourceFile { get; set; }

#if !(SCHEMAGENERATOR || CLI_TOOL)
        public static ExpectedAppConfig FromTaskItem(Microsoft.Build.Framework.ITaskItem item) =>
            new ExpectedAppConfig
            {
                OutputPath = item.ItemSpec,
                SourceFile = item.GetMetadata(BuildTools.Configuration.MetaData.SourceFile)
            };
#endif
    }
}
