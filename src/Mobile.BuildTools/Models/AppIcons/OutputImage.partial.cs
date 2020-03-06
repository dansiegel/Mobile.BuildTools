#if !NETCOREAPP
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Mobile.BuildTools.Models.AppIcons
{

    public partial class OutputImage
    {
        public ITaskItem ToTaskItem()
        {
            var item = new TaskItem(OutputFile);
            item.SetMetadata("InputFile", InputFile);
            item.SetMetadata("OutputLink", OutputLink);
            item.SetMetadata("Height", Height.ToString());
            item.SetMetadata("Width", Width.ToString());
            item.SetMetadata("RequiresBackgroundColor", RequiresBackgroundColor.ToString());
            item.SetMetadata("Scale", Scale.ToString());
            item.SetMetadata("ShouldBeVisible", ShouldBeVisible.ToString());
            item.SetMetadata("WatermarkFilePath", WatermarkFilePath);
            item.SetMetadata("BackgroundColor", BackgroundColor);

            return item;
        }

        public static OutputImage FromTaskItem(ITaskItem item)
        {
            var image = new OutputImage
            {
                InputFile = item.GetMetadata("InputFile"),
                OutputFile = item.ItemSpec,
                OutputLink = item.GetMetadata("OutputLink"),
                WatermarkFilePath = item.GetMetadata("WatermarkFilePath"),
                BackgroundColor = item.GetMetadata("BackgroundColor")
            };

            int.TryParse(item.GetMetadata("Height"), out var height);
            int.TryParse(item.GetMetadata("Width"), out var width);
            bool.TryParse(item.GetMetadata("RequiresBackgroundColor"), out var requiresBackgroundColor);
            bool.TryParse(item.GetMetadata("ShouldBeVisible"), out var shouldBeVisible);
            if(double.TryParse(item.GetMetadata("Scale"), out var scale) && (height == 0 || width == 0) && scale == 0)
            {
                scale = 1;
            }

            image.Height = height;
            image.Width = width;
            image.RequiresBackgroundColor = requiresBackgroundColor;
            image.ShouldBeVisible = shouldBeVisible;
            image.Scale = scale;

            return image;
        }
    }
}
#endif
