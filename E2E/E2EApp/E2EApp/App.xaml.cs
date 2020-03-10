using System.IO;
using System.Text.RegularExpressions;
using E2EApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace E2EApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var tabbedPage = new TabbedPage();
            tabbedPage.Children.Add(new SecretsPage());
            tabbedPage.Children.Add(new AppConfig());
            tabbedPage.Children.Add(new ImagesPage());
            MainPage = tabbedPage;
        }
    }
}
