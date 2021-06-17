namespace Mobile.BuildTools.Models.Configuration
{
    public static class ExpectedAppConfigExtensions
    {
        public static ExpectedAppConfig ToExpectedAppConfig(this Microsoft.Build.Framework.ITaskItem item) =>
            new ExpectedAppConfig
            {
                OutputPath = item.ItemSpec,
                SourceFile = item.GetMetadata(BuildTools.Configuration.MetaData.SourceFile)
            };
    }
}
