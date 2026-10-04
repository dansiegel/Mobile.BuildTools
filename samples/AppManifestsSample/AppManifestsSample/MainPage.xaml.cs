namespace AppManifestsSample;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        ManifestSummary.Text = $"{AppInfo.Current.Name}\n{AppInfo.Current.PackageName}";
    }
}
