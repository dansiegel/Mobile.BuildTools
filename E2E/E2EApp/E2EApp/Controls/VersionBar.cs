using Xamarin.Essentials;
using Xamarin.Forms;

namespace E2EApp.Controls
{
    public class VersionBar : StackLayout
    {
        public VersionBar()
        {
            Children.Add(new Label
            {
                Text = $"BuildString: {AppInfo.BuildString}",
                TextColor = Color.White,
                AutomationId = "BuildStringLabel"
            });
            Children.Add(new Label
            {
                Text = $"VersionString: {AppInfo.VersionString}", 
                TextColor = Color.White,
                AutomationId = "VersionStringLabel"
            });

            Spacing = 20;
            Padding = 20;
            BackgroundColor = Color.Black;
            Orientation = StackOrientation.Horizontal;
            HorizontalOptions = LayoutOptions.Center;
        }
    }
}
